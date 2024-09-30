using System;
using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using Unity.AI.Navigation;

using UnityEngine;
using UnityEngine.AI;

namespace BC.Map
{
	public class NavMeshConnectComputer : ComponentBehaviour
	{
		private MapCellData mapCellData;
		public bool usingNavMeshRaycast;
		[ShowIf("usingNavMeshRaycast")]
		public int areaMask = NavMesh.AllAreas;
		private int physicsLayerMask = -1;

		internal List<Triangle> triangleList;
		internal List<LinkRayTriangle> linkRayTriangleList;

		//Temp List
		private List<Triangle> asyncTriangleList;
		private List<LinkRayTriangle> asyncLinkRayTriangleList;
		private Dictionary<Vector3Int,List<Triangle>> asyncTrianglesTile = new Dictionary<Vector3Int,List<Triangle>>();

		Coroutine asyncUpdate;
		public float tiemForCalculationCount = 1/10;
		private DateTime previousTime;
		[SerializeField,ReadOnly]
		private bool isAsyncUpdate;
		public bool IsAsyncUpdate => isAsyncUpdate && !DisableCancelToken.IsCancellationRequested;
		//#if UNITY_EDITOR
		//			editorCoroutine != null ||
		//#endif
		//			asyncUpdate != null;

		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<MapCellData>(out var cellIndexData))
			{
				cellIndexData = ThisContainer.AddData<MapCellData>();
			}
			cellIndexData.trianglesTile = new Dictionary<Vector3Int, List<Triangle>>();
			cellIndexData.trianglesToLink = new Dictionary<Triangle, LinkRayTriangle[]>();
			cellIndexData.trianglesClosedPoint = new Dictionary<Triangle, IMapPathPoint>();
			cellIndexData.navMeshSurface = GetComponentInChildren<NavMeshSurface>();
		}

		public override void BaseAwake()
		{
			if(!ThisContainer.TryGetData<MapCellData>(out var cellIndexData))
			{
				cellIndexData = ThisContainer.AddData<MapCellData>();
			}
			cellIndexData.trianglesTile = new Dictionary<Vector3Int, List<Triangle>>();
			cellIndexData.trianglesToLink = new Dictionary<Triangle, LinkRayTriangle[]>();
			cellIndexData.trianglesClosedPoint = new Dictionary<Triangle, IMapPathPoint>();
			cellIndexData.navMeshSurface = GetComponentInChildren<NavMeshSurface>();

			cellIndexData.navMeshSurface.BuildNavMesh();
		}
		public override void BaseDestroy()
		{
			if(IsAsyncUpdate)
			{
				StopCoroutine(asyncUpdate);
				asyncUpdate = null;
			}
		}

		public override void BaseEnable()
		{
			triangleList = new List<Triangle>();
			linkRayTriangleList = new List<LinkRayTriangle>();
#if UNITY_EDITOR
			editorCoroutine = null;
#endif
			if(!isAsyncUpdate)
			{
				AsyncUpdate();
			}
		}
		public override void BaseDisable()
		{
			isAsyncUpdate = false;
		}

		public async void AsyncUpdate()
		{
			isAsyncUpdate = true;
			mapCellData = ThisContainer.GetData<MapCellData>();
			physicsLayerMask = TagAndLayer.GetHitLayerMask(TagAndLayer.NavMeshRaycast);

			await Awaitable.NextFrameAsync();
			if(IsAsyncUpdate)
			{
				await InitTriangleList();
			}
			if(IsAsyncUpdate)
			{
				await BuindingAllTriangle();
			}
			if(IsAsyncUpdate)
			{
				triangleList.Clear();
				linkRayTriangleList.Clear();
				triangleList.AddRange(asyncTriangleList);
				linkRayTriangleList.AddRange(asyncLinkRayTriangleList);
			}
			if(IsAsyncUpdate)
			{
				await CashingTrianglesTile();
			}
			if(IsAsyncUpdate)
			{
				await CashingTrianglesToLink();
			}
			if(IsAsyncUpdate)
			{
				await CashingTrianglesClosedPoint();
			}
			if(IsAsyncUpdate)
			{
				await ClearIsolatedTriangles();
			}
			asyncTrianglesTile.Clear();
			asyncTriangleList.Clear();
			asyncLinkRayTriangleList.Clear();
			isAsyncUpdate = false;
		}
		private async Awaitable InitTriangleList()
		{
			NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
			await Awaitable.BackgroundThreadAsync();

			asyncTriangleList ??= new List<Triangle>();
			asyncTriangleList.Clear();

			// NavMesh의 삼각형 배열을 가져옴
			int[] indices = navMeshData.indices;
			int Length = indices.Length;
			if(Length == 0) return;

			// NavMesh의 삼각형을 통해 정점을 가져옴
			Vector3[] vertices = navMeshData.vertices;

			// 각 삼각형의 정점을 순회하여 출력

			for(int i = 0 ; i < Length ; i += 3)
			{
				Vector3 vertex1 = vertices[indices[i]];
				Vector3 vertex2 = vertices[indices[i + 1]];
				Vector3 vertex3 = vertices[indices[i + 2]];
				Triangle triangle = new Triangle(asyncTriangleList.Count, vertex1 , vertex2, vertex3);
				asyncTriangleList.Add(triangle);
			}

			asyncTrianglesTile ??= new Dictionary<Vector3Int, List<Triangle>>();
			asyncTrianglesTile.Clear();

			int Count = asyncTriangleList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var triangle = asyncTriangleList[i];
				Vector3 center = triangle.Center;

				Vector3Int titlePos = mapCellData.VectorToIndex(center);
				if(!asyncTrianglesTile.TryGetValue(titlePos, out var titleValue))
				{
					titleValue = new List<Triangle>();
					asyncTrianglesTile.Add(titlePos, titleValue);
				}
				titleValue.Add(triangle);

			}
			await Awaitable.MainThreadAsync();
		}
		private async Awaitable BuindingAllTriangle()
		{
			asyncLinkRayTriangleList ??= new List<LinkRayTriangle>();
			asyncLinkRayTriangleList.Clear();

			int Count = asyncTriangleList.Count;

			for(int i = 0 ; i < Count ; i++)
			{
				var triangleA = asyncTriangleList[i];
				for(int ii = i + 1 ; ii < Count ; ii++)
				{
					var triangleB = asyncTriangleList[ii];

					LinkRayTriangle.LinkType linkType = Triangle.CheckSpaceBetweenTriangle(triangleA, triangleB, 0.01f, (Vector3 a, Vector3 b) =>
					{
						if(usingNavMeshRaycast)
						{
							NavMeshHit hit = default;

							if (NavMesh.SamplePosition(a, out  hit, 0.01f, areaMask))
							{
								a = hit.position;
							}
							if (NavMesh.SamplePosition(b, out  hit, 0.01f, areaMask))
							{
								b = hit.position;
							}

							return NavMesh.Raycast(a, b, out hit, areaMask);
						}
						else
						{
							b += Vector3.up;
							a += Vector3.up;

							Vector3 direction = b - a;
							float distance = direction.magnitude;
							direction = direction.normalized;
							Ray ray1 = new Ray(a, direction);
							Ray ray2 = new Ray(b, -direction);
							bool isHit =
							Physics.Raycast(ray1, out _, distance, physicsLayerMask, QueryTriggerInteraction.Ignore) ||
							Physics.Raycast(ray2, out _, distance, physicsLayerMask, QueryTriggerInteraction.Ignore);
							return isHit;
						}
					});

					if(linkType != LinkRayTriangle.LinkType.NothingLink)
					{
						var linkTriangle = new LinkRayTriangle(linkType , triangleA, triangleB);
						asyncLinkRayTriangleList.Add(linkTriangle);
					}

					if(IsAsyncUpdate)
					{
						if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
						{
							previousTime = DateTime.Now;
							await Awaitable.NextFrameAsync(DisableCancelToken);
						}
					}
					else
					{
						return;
					}
				}
			}
		}
		private async Awaitable CashingTrianglesTile()
		{
			await Awaitable.BackgroundThreadAsync();

			mapCellData.trianglesTile.Clear();
			foreach(var key in asyncTrianglesTile)
			{
				mapCellData.trianglesTile.Add(key.Key, key.Value);
			}

			await Awaitable.MainThreadAsync();
		}
		private async Awaitable CashingTrianglesToLink()
		{
			await Awaitable.BackgroundThreadAsync();

			mapCellData.trianglesToLink.Clear();
			foreach(Triangle triangle in asyncTriangleList)
			{
				List<LinkRayTriangle> linkList = new List<LinkRayTriangle>();
				foreach(LinkRayTriangle link in asyncLinkRayTriangleList)
				{
					if(link.IsThis(triangle.Index))
					{
						linkList.Add(link);
					}
				}
				mapCellData.trianglesToLink.Add(triangle, linkList.ToArray());
			}

			await Awaitable.MainThreadAsync();
		}
		private async Awaitable CashingTrianglesClosedPoint()
		{
			mapCellData.trianglesClosedPoint.Clear();
			MapAnchor[] mapAnchors = ThisContainer.GetChildAllObject<MapAnchor>();
			for(int i = 0 ; i<asyncTriangleList.Count ; i++)
			{
				Triangle triangle = asyncTriangleList[i];
				NavMeshHit hit = default;
				NavMeshPath path = new NavMeshPath();

				Vector3 center = triangle.Center;
				if(NavMesh.SamplePosition(center, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
				{
					center = hit.position;
				}
				float minDistance = float.MaxValue;
				MapPathPoint closePathPoint = null;
				for(int ii = 0 ; ii < mapAnchors.Length ; ii++)
				{
					MapAnchor anchor = mapAnchors[ii];
					if(anchor.ThisContainer.TryGetComponent<MapPathPoint>(out var pathPoint))
					{
						Vector3 closePoint = pathPoint.ThisPosition();
						if(NavMesh.SamplePosition(closePoint, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
						{
							closePoint = hit.position;
						}
						if(NavMesh.CalculatePath(center, closePoint, NavMesh.AllAreas, path))
						{
							if(path.status == NavMeshPathStatus.PathComplete)
							{
								float totalDistance = 0f;
								for(int iii = 0 ; iii < path.corners.Length - 1 ; iii++)
								{
									totalDistance += Vector3.Distance(path.corners[iii], path.corners[iii + 1]);
								}
								if(minDistance > totalDistance)
								{
									minDistance = totalDistance;
									closePathPoint = pathPoint;
								}

								closePoint = pathPoint.ClosestPoint(center, out _);
								if(NavMesh.SamplePosition(closePoint, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
								{
									closePoint = hit.position;
								}

								if(NavMesh.CalculatePath(center, closePoint, NavMesh.AllAreas, path))
								{
									if(path.status == NavMeshPathStatus.PathComplete)
									{
										totalDistance = 0f;
										for(int iii = 0 ; iii < path.corners.Length - 1 ; iii++)
										{
											totalDistance += Vector3.Distance(path.corners[iii], path.corners[iii + 1]);
										}
										if(minDistance > totalDistance)
										{
											minDistance = totalDistance;
											closePathPoint = pathPoint;
										}
									}
								}
							}
						}
						if(IsAsyncUpdate)
						{
							if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
							{
								previousTime = DateTime.Now;
								await Awaitable.NextFrameAsync(DisableCancelToken);
							}
						}
						else
						{
							return;
						}
					}
				}
				if(closePathPoint != null)
				{
					mapCellData.trianglesClosedPoint.Add(triangle, closePathPoint);
				}
			}
		}
		private async Awaitable ClearIsolatedTriangles()
		{
			await Awaitable.BackgroundThreadAsync();

			Dictionary<Triangle, IMapPathPoint> trianglesClosedPoint = mapCellData.trianglesClosedPoint;
			var tileList = mapCellData.trianglesTile;
			var linkList = mapCellData.trianglesToLink;

			foreach(var item in tileList)
			{
				var list = item.Value;

				for(int i = 0 ; i < list.Count ; i++)
				{
					var triangle = list[i];
					if(!trianglesClosedPoint.ContainsKey(triangle))
					{
						list.RemoveAt(i--);
					}
				}
			}

			{
				List<Triangle> removeList = new List<Triangle>();
				foreach(var item in linkList)
				{
					var triangle = item.Key;
					if(!trianglesClosedPoint.ContainsKey(triangle))
					{
						removeList.Add(triangle);
					}
				}
				int count = removeList.Count;
				for(int i = 0 ; i < count ; i++)
				{
					linkList.Remove(removeList[i]);
				}
			}

			{
				triangleList = triangleList.Where(item
					=> trianglesClosedPoint.ContainsKey(item)).ToList();

				asyncLinkRayTriangleList = asyncLinkRayTriangleList.Where(item
					=> trianglesClosedPoint.ContainsKey(item.linkTriangleA)
					|| trianglesClosedPoint.ContainsKey(item.linkTriangleB)).ToList();
			}

			await Awaitable.MainThreadAsync();
		}
		public bool FindPointInTriangle(Vector3 point, out Triangle triangle)
		{
			if(NavMesh.SamplePosition(point, out var hit, 0.01f, areaMask))
			{
				point = hit.position;
			}

			Vector3Int titleIndex = mapCellData.VectorToIndex(point);
			if(!mapCellData.trianglesTile.TryGetValue(titleIndex, out List<Triangle> titleValue))
			{
				titleValue = triangleList;
			}

			int Count = titleValue.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var find = titleValue[i];
				if(find.IsPointInTriangle(point))
				{
					triangle = find;
					return true;
				}
			}
			triangle = default;
			return false;
		}
		public bool FindPointInTriangle(Vector3 point, out Triangle triangle, out LinkRayTriangle[] linkTriangles)
		{
			if(NavMesh.SamplePosition(point, out var hit, 0.01f, areaMask))
			{
				point = hit.position;
			}

			Vector3Int titleIndex = mapCellData.VectorToIndex(point);
			if(!mapCellData.trianglesTile.TryGetValue(titleIndex, out List<Triangle> titleValue))
			{
				titleValue = triangleList;
			}

			int Count = titleValue.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var find = titleValue[i];
				if(find.IsPointInTriangle(point))
				{
					triangle = find;
					if(mapCellData.trianglesToLink.TryGetValue(find, out var linkList))
					{
						linkTriangles = linkList;
					}
					else
					{
						linkTriangles = linkRayTriangleList.Where(item => item.IsThis(find.Index)).ToArray();
					}
					return linkTriangles.Length > 0;
				}
			}
			triangle = default;
			linkTriangles = null;
			return false;
		}
		public bool FindLinkTriangle(Triangle A, Triangle B, out LinkRayTriangle linkTriangles)
		{
			linkTriangles = default;
			foreach(var link in linkRayTriangleList)
			{
				if(link.IsThis(A.Index, B.Index))
				{
					linkTriangles = link;
					return true;
				}
			}
			return false;
		}
		public bool FindClosePathPoint(Vector3 point, out IMapPathPoint mapPathPoint)
		{
			Vector3Int titleIndex = mapCellData.VectorToIndex(point);
			if(!mapCellData.trianglesTile.TryGetValue(titleIndex, out List<Triangle> titleValue))
			{
				titleValue = triangleList;
			}

			int Count = titleValue.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var find = titleValue[i];
				if(find.IsPointInTriangle(point))
				{
					return FindClosePathPoint(find, out mapPathPoint);
				}
			}
			mapPathPoint = null;
			return false;
		}
		public bool FindClosePathPoint(Triangle triangle, out IMapPathPoint mapPathPoint)
		{
			if(mapCellData.trianglesClosedPoint.TryGetValue(triangle, out mapPathPoint))
			{
				return true;
			}
			else
			{
				MapAnchor[] mapAnchors = ThisContainer.GetChildAllObject<MapAnchor>();
				NavMeshHit hit = default;
				NavMeshPath path = new NavMeshPath();

				Vector3 center = triangle.Center;
				if(NavMesh.SamplePosition(center, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
				{
					center = hit.position;
				}
				float minDistance = float.MaxValue;
				MapPathPoint closePathPoint = null;
				for(int ii = 0 ; ii < mapAnchors.Length ; ii++)
				{
					MapAnchor anchor = mapAnchors[ii];
					if(anchor.ThisContainer.TryGetComponent<MapPathPoint>(out var pathPoint))
					{
						Vector3 closePoint = pathPoint.ThisPosition();
						if(NavMesh.SamplePosition(closePoint, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
						{
							closePoint = hit.position;
						}
						if(NavMesh.CalculatePath(center, closePoint, NavMesh.AllAreas, path))
						{
							if(path.status == NavMeshPathStatus.PathComplete)
							{
								float totalDistance = 0f;
								for(int iii = 0 ; iii < path.corners.Length - 1 ; iii++)
								{
									totalDistance += Vector3.Distance(path.corners[iii], path.corners[iii + 1]);
								}
								if(minDistance > totalDistance)
								{
									minDistance = totalDistance;
									closePathPoint = pathPoint;
								}

								closePoint = pathPoint.ClosestPoint(center, out _);
								if(NavMesh.SamplePosition(closePoint, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
								{
									closePoint = hit.position;
								}
								if(NavMesh.CalculatePath(center, closePoint, NavMesh.AllAreas, path))
								{
									if(path.status == NavMeshPathStatus.PathComplete)
									{
										totalDistance = 0f;
										for(int iii = 0 ; iii < path.corners.Length - 1 ; iii++)
										{
											totalDistance += Vector3.Distance(path.corners[iii], path.corners[iii + 1]);
										}
										if(minDistance > totalDistance)
										{
											minDistance = totalDistance;
											closePathPoint = pathPoint;
										}
									}
								}
							}
						}
					}
				}
				mapPathPoint = closePathPoint;
			}
			return mapPathPoint != null;
		}

#if UNITY_EDITOR
		private Unity.EditorCoroutines.Editor.EditorCoroutine editorCoroutine = null;
		[Button]
		private void UpdateInEditor()
		{
			if(editorCoroutine != null)
			{
				Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StopCoroutine(editorCoroutine);
			}
			triangleList = new List<Triangle>();
			linkRayTriangleList = new List<LinkRayTriangle>();
			//editorCoroutine = Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(AsyncUpdate(), this);
		}

		[Header("OnDrawGizmos")]
		[SerializeField]
		private bool IsOnDrawGizmos;
		[SerializeField, ShowIf("@IsOnDrawGizmos")]
		private bool IsOnDrawGizmos_ShowPerfectLink;
		[SerializeField, ShowIf("@IsOnDrawGizmos")]
		private bool IsOnDrawGizmos_ShowPartialLink;

		[SerializeField,ReadOnly, ShowIf("@IsOnDrawGizmos")]
		private int onMouseTriangleIndex;
		[SerializeField,ReadOnly, ShowIf("@IsOnDrawGizmos")]
		private Vector3 onMouseTrianglePosition;
		[SerializeField,ReadOnly, ShowIf("@IsOnDrawGizmos")]
		private Vector3 onMouseTriangleNavPosition;
		[SerializeField, ShowIf("@IsOnDrawGizmos")]
		private Vector3 onGizmosTargetPosition;
		[SerializeField, ReadOnly, ShowIf("@IsOnDrawGizmos")]
		private bool onGizmosTargetPositionConnect;
		public void OnDrawGizmos()
		{
			onMouseTriangleIndex = 0;
			onMouseTrianglePosition = Vector3.zero;
			onMouseTriangleNavPosition = Vector3.zero;
			onGizmosTargetPositionConnect = false;

			if(!IsOnDrawGizmos) return;
			if(triangleList == null) return;
			if(linkRayTriangleList == null) return;

			if(UnityEditor.SceneView.currentDrawingSceneView == null) return;

			Vector3 mousePosition = Event.current.mousePosition;
			mousePosition.y = UnityEditor.SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
			Ray ray = UnityEditor.SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePosition);

			int Count = triangleList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var triangle = triangleList[i];
				Plane trianglePlane = triangle.GetPlane;

				// Ray와 삼각형의 평면이 교차하는지 확인
				bool isHit = false;
				if(trianglePlane.Raycast(ray, out float enter))
				{
					Vector3 intersectionPoint = ray.GetPoint(enter);

					// 교차점이 삼각형 내부에 있는지 확인
					if(triangle.IsPointInTriangle(intersectionPoint))
					{
						// 교차하는 삼각형을 초록색으로 그림
						isHit = true;
					}
				}
				if(isHit)
				{
					Gizmos.color = Color.green;
					Vector3 offset = Vector3.zero;
					for(int ii = 0 ; ii < 25 ; ii++)
					{
						offset += Vector3.up * 0.04f;
						triangle.OnDrawGizmos(offset);
					}
					Vector3 center = triangle.Center;
					Gizmos.DrawSphere(center + offset, 0.5f);

					if(mapCellData != null)
					{
						if(mapCellData.trianglesToLink.TryGetValue(triangle, out var thisLinkTriangle))
						{
							foreach(var link in thisLinkTriangle)
							{
								if(link.IsPerfectLink && IsOnDrawGizmos_ShowPerfectLink)
								{
									Gizmos.color = Color.green;
									link.OnDrawGizmos(offset, 0.25f);
								}
								else if(!link.IsPerfectLink && IsOnDrawGizmos_ShowPartialLink)
								{
									Gizmos.color = Color.yellow;
									link.OnDrawGizmos(offset, 0.25f);
								}
							}
						}
						if(mapCellData.trianglesClosedPoint.TryGetValue(triangle, out var mapPathPoint))
						{
							Gizmos.color = Color.blue;
							for(int ii = 0 ; ii < 25 ; ii++)
							{
								offset += Vector3.up * 0.04f;
								Gizmos.DrawLine(mapPathPoint.ThisPosition() + offset, center + offset);
							}
						}
					}
					onMouseTriangleIndex = triangle.Index;
					onMouseTrianglePosition = triangle.Center;
					if(NavMesh.SamplePosition(onMouseTrianglePosition, out var hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
					{
						onMouseTriangleNavPosition = hit.position;
					}
					Gizmos.DrawSphere(onGizmosTargetPosition + Vector3.up * 2, 0.5f);
					if(NavMesh.SamplePosition(onGizmosTargetPosition, out hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
					{
						onGizmosTargetPosition = hit.position;
					}
					Gizmos.color=Color.black;
					Gizmos.DrawSphere(onGizmosTargetPosition  + Vector3.up * 1.5f, 0.5f);
					NavMeshPath navMeshPath = new NavMeshPath();
					if(NavMesh.CalculatePath(onMouseTriangleNavPosition, onGizmosTargetPosition, NavMesh.AllAreas, navMeshPath))
					{
						if(navMeshPath.status == NavMeshPathStatus.PathComplete)
						{
							onGizmosTargetPositionConnect = true;
							for(int ii = 0 ; ii<navMeshPath.corners.Length -1 ; ii++)
							{
								Vector3 item1 = navMeshPath.corners[ii];
								Vector3 item2 = navMeshPath.corners[ii+1];

								Gizmos.DrawLine(item1, item2);
							}
						}
					}
					UnityEditor.Handles.Label(onMouseTrianglePosition, $"::{onMouseTriangleIndex}", new GUIStyle() {
						fontSize = Mathf.RoundToInt(30),
						normal = new GUIStyleState() { textColor = Color.red }
					});
				}
				else
				{
					Gizmos.color = Color.black;
					Vector3 offset = Vector3.up;
					triangle.OnDrawGizmos(offset);
				}
			}
		}
#endif
	}
}
