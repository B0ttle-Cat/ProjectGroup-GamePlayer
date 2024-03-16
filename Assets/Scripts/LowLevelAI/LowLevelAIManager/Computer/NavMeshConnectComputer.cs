using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using Unity.AI.Navigation;




#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

using UnityEngine;
using UnityEngine.AI;

using Debug = UnityEngine.Debug;

namespace BC.LowLevelAI
{
	public class NavMeshConnectComputer : ComponentBehaviour
	{
		private MapAICellData mapAICellData;

#if UNITY_EDITOR
		[SerializeField]
		private bool IsOnDrawGizmos;
		[SerializeField]
		private bool IsOnDrawGizmos_ShowOnlyPerfectLink;
#endif

		#region Triangle
		public struct Triangle
		{
			public int Index;
			public Vector3 vertex1;
			public Vector3 vertex2;
			public Vector3 vertex3;

			public Triangle(int index, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
			{
				Index = index;
				this.vertex1=vertex1;
				this.vertex2=vertex2;
				this.vertex3=vertex3;
			}

			public Vector3 Center()
			{
				// 각 좌표의 평균값을 계산하여 중심점을 얻음
				Vector3 center = (vertex1 + vertex2 + vertex3) / 3f;
				return center;
			}
			public Plane GetPlane => new Plane(vertex1, vertex2, vertex3);
			public bool IsPointInTriangle(Vector3 point)
			{
				// 각 점으로부터 벡터를 만들고 외적을 사용하여 삼각형의 노멀을 구함
				Vector3 u = vertex2 - vertex1;
				Vector3 v = vertex3 - vertex1;
				Vector3 w = point - vertex1;
				Vector3 n = Vector3.Cross(u, v);

				// 외적이 0이면 삼각형이 아님
				if(n == Vector3.zero)
					return false;

				// 각 점에서의 삼각형의 면과의 거리를 구함
				float gamma = Vector3.Dot(Vector3.Cross(u, w), n) / n.sqrMagnitude;
				float beta = Vector3.Dot(Vector3.Cross(w, v), n) / n.sqrMagnitude;
				float alpha = 1.0f - gamma - beta;

				// 모든 거리가 양수이면 점은 삼각형 내부에 있음
				return alpha > 0 && beta > 0 && gamma > 0;
			}
			public void OnDrawGizmos(Vector3 offset)
			{
#if UNITY_EDITOR
				Gizmos.DrawLine(vertex1 + offset, vertex2 + offset);
				Gizmos.DrawLine(vertex2 + offset, vertex3 + offset);
				Gizmos.DrawLine(vertex3 + offset, vertex1 + offset);
#endif
			}
			public void Log()
			{
#if UNITY_EDITOR
				Debug.Log($"Triangle ({Index}) : v1({vertex1}) v2({vertex2}) v3({vertex3})");
#endif
			}

			public static int CheckSpaceBetweenTriangle(Triangle A, Triangle B, float minimumDistance, Func<Vector3, Vector3, bool> raycast)
			{
				if(raycast is null) return 0;

				int count = 0;
				count += (Vector3.Distance(A.vertex1, B.vertex1) <= minimumDistance || raycast(A.vertex1, B.vertex1)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex1, B.vertex2) <= minimumDistance || raycast(A.vertex1, B.vertex2)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex1, B.vertex3) <= minimumDistance || raycast(A.vertex1, B.vertex3)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex2, B.vertex1) <= minimumDistance || raycast(A.vertex2, B.vertex1)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex2, B.vertex2) <= minimumDistance || raycast(A.vertex2, B.vertex2)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex2, B.vertex3) <= minimumDistance || raycast(A.vertex2, B.vertex3)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex3, B.vertex1) <= minimumDistance || raycast(A.vertex3, B.vertex1)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex3, B.vertex2) <= minimumDistance || raycast(A.vertex3, B.vertex2)) ? 1 : 0;
				count += (Vector3.Distance(A.vertex3, B.vertex3) <= minimumDistance || raycast(A.vertex3, B.vertex3)) ? 1 : 0;

				return count;
			}
		}
		public struct LinkTriangle
		{
			public int linkCount;
			public Triangle linkTriangleA;
			public Triangle linkTriangleB;

			public LinkTriangle(int linkCount, Triangle linkTriangleA, Triangle linkTriangleB)
			{
				this.linkCount = linkCount;
				this.linkTriangleA=linkTriangleA;
				this.linkTriangleB=linkTriangleB;
			}

			public bool IsThis(int A)
			{
				return (linkTriangleA.Index == A || linkTriangleB.Index == A);
			}

			public bool IsThis(int A, int B)
			{
				return A != B &&
					(linkTriangleA.Index == A || linkTriangleB.Index == A) &&
					(linkTriangleA.Index == B || linkTriangleB.Index == B);
			}
			public void Log()
			{
				Debug.Log($"LinkTriangle : {linkTriangleA.Index} == {linkTriangleB.Index} : Link ({linkCount})");
			}
			public bool IsPerfectLink => linkCount == 9;
			internal void OnDrawGizmos(Vector3 offset, float radius)
			{
				Vector3 a = linkTriangleA.Center() + offset;
				Vector3 b = linkTriangleB.Center() + offset;

				Gizmos.DrawSphere(a, radius);
				Gizmos.DrawSphere(b, radius);
				Gizmos.DrawLine(a, b);
			}

			public bool GetOther(Triangle triangle, out Triangle other)
			{
				if(linkTriangleA.Index == triangle.Index)
				{
					other = linkTriangleB;
					return true;
				}
				else if(linkTriangleB.Index == triangle.Index)
				{
					other = linkTriangleA;
					return true;
				}
				other = default;
				return false;
			}
		}
		#endregion

		private double tiemForCalculationCount = 1d/30d;
		private DateTime previousTime;

		public bool usingNavMeshRaycast;
		[ShowIf("usingNavMeshRaycast")]
		public int areaMask = NavMesh.AllAreas;


		internal List<Triangle> triangleList;
		internal List<LinkTriangle> linkTriangleList;
		private List<Triangle> asyncTriangleList;
		private List<LinkTriangle> asyncLinkTriangleList;
		Coroutine asyncUpdate;

		public bool IsAsyncUpdate =>
#if UNITY_EDITOR
			editorCoroutine != null ||
#endif
			asyncUpdate != null;


		private Dictionary<Vector3Int,List<Triangle>> asyncTrianglesTile = new Dictionary<Vector3Int,List<Triangle>>();

		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<MapAICellData>(out var mapAICellData))
			{
				mapAICellData = ThisContainer.AddData<MapAICellData>();
			}
			mapAICellData.navMeshSurface = GetComponentInChildren<NavMeshSurface>();
		}

		public override void BaseAwake()
		{

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
			linkTriangleList = new List<LinkTriangle>();
#if UNITY_EDITOR
			editorCoroutine = null;
#endif
			if(IsAsyncUpdate)
			{
				StopCoroutine(asyncUpdate);
			}
			asyncUpdate = StartCoroutine(AsyncUpdate());
		}

		public IEnumerator AsyncUpdate()
		{
			mapAICellData = ThisContainer.GetData<MapAICellData>();

			previousTime = DateTime.Now;
			yield return null;
			Debug.Log("Start WaypointConnectUpdate");
			if(IsAsyncUpdate)
			{
				yield return InitTriangleList();
			}
			if(IsAsyncUpdate)
			{
				yield return BuindingAllTriangle();
			}
			if(IsAsyncUpdate)
			{
				triangleList.Clear();
				linkTriangleList.Clear();
				triangleList.AddRange(asyncTriangleList);
				linkTriangleList.AddRange(asyncLinkTriangleList);

				Debug.Log("Cashing WaypointConnectUpdate");
				yield return CashingTrianglesTile();

				yield return CashingTrianglesToLink();

				asyncTrianglesTile.Clear();
				asyncTriangleList.Clear();
				asyncLinkTriangleList.Clear();
			}
			asyncUpdate = null;
			Debug.Log("Ended WaypointConnectUpdate");
		}

		private IEnumerator InitTriangleList()
		{
			NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

			asyncTriangleList ??= new List<Triangle>();
			asyncTriangleList.Clear();

			// NavMesh의 삼각형 배열을 가져옴
			int[] indices = navMeshData.indices;
			int Length = indices.Length;
			if(Length == 0) yield break;

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
				triangle.Log();

				if(IsAsyncUpdate)
				{
					if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
					{
						previousTime = DateTime.Now;
						yield return null;
					}
				}
				else
				{
					yield break;
				}
			}

			asyncTrianglesTile ??= new Dictionary<Vector3Int, List<Triangle>>();
			asyncTrianglesTile.Clear();

			int Count = asyncTriangleList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var triangle = asyncTriangleList[i];
				Vector3 center = triangle.Center();

				Vector3Int titlePos = mapAICellData.GetCellIndex(center);
				if(!asyncTrianglesTile.TryGetValue(titlePos, out var titleValue))
				{
					titleValue = new List<Triangle>();
					asyncTrianglesTile.Add(titlePos, titleValue);
				}
				titleValue.Add(triangle);

				if(IsAsyncUpdate)
				{
					if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
					{
						previousTime = DateTime.Now;
						yield return null;
					}
				}
				else
				{
					yield break;
				}
			}
		}
		private IEnumerator BuindingAllTriangle()
		{
			asyncLinkTriangleList ??= new List<LinkTriangle>();
			asyncLinkTriangleList.Clear();

			int Count = asyncTriangleList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var triangleA = asyncTriangleList[i];
				for(int ii = i + 1 ; ii < Count ; ii++)
				{
					var triangleB = asyncTriangleList[ii];

					int linkCount = Triangle.CheckSpaceBetweenTriangle(triangleA, triangleB, 0.01f, (Vector3 a, Vector3 b) =>
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

							return !NavMesh.Raycast(a, b, out hit, areaMask);
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
							Physics.Raycast(ray1, out _, distance, TagAndLayer.GetHitLayerMask(TagAndLayer.NavMeshRaycast), QueryTriggerInteraction.Ignore) ||
							Physics.Raycast(ray2, out _, distance, TagAndLayer.GetHitLayerMask(TagAndLayer.NavMeshRaycast), QueryTriggerInteraction.Ignore);
							return !isHit;
						}
					});

					if(linkCount > 0)
					{
						var linkTriangle = new LinkTriangle(linkCount, triangleA, triangleB);
						asyncLinkTriangleList.Add(linkTriangle);
						linkTriangle.Log();
					}

					if(IsAsyncUpdate)
					{
						if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
						{
							previousTime = DateTime.Now;
							yield return null;
						}
					}
					else
					{
						yield break;
					}
				}
			}
		}
		private IEnumerator CashingTrianglesTile()
		{
			mapAICellData.trianglesTile.Clear();
			foreach(var key in asyncTrianglesTile)
			{
				mapAICellData.trianglesTile.Add(key.Key, key.Value);
				if(IsAsyncUpdate)
				{
					if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
					{
						previousTime = DateTime.Now;
						yield return null;
					}
				}
				else
				{
					yield break;
				}
			}
		}
		private IEnumerator CashingTrianglesToLink()
		{
			mapAICellData.trianglesToLink.Clear();
			foreach(Triangle triangle in asyncTriangleList)
			{
				List<LinkTriangle> linkList = new List<LinkTriangle>();
				foreach(LinkTriangle link in asyncLinkTriangleList)
				{
					if(link.IsThis(triangle.Index))
					{
						linkList.Add(link);
					}
					if(IsAsyncUpdate)
					{
						if((DateTime.Now - previousTime).TotalSeconds >= tiemForCalculationCount)
						{
							previousTime = DateTime.Now;
							yield return null;
						}
					}
					else
					{
						yield break;
					}
				}
				mapAICellData.trianglesToLink.Add(triangle, linkList.ToArray());
			}
		}

		public bool FindPointInTriangle(Vector3 point, out Triangle triangle)
		{
			if(NavMesh.SamplePosition(point, out var hit, 0.01f, areaMask))
			{
				point = hit.position;
			}

			Vector3Int titleIndex = mapAICellData.GetCellIndex(point);
			if(!mapAICellData.trianglesTile.TryGetValue(titleIndex, out List<Triangle> titleValue))
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
		public bool FindPointInTriangle(Vector3 point, out Triangle triangle, out LinkTriangle[] linkTriangles)
		{
			if(NavMesh.SamplePosition(point, out var hit, 0.01f, areaMask))
			{
				point = hit.position;
			}

			Vector3Int titleIndex = mapAICellData.GetCellIndex(point);
			if(!mapAICellData.trianglesTile.TryGetValue(titleIndex, out List<Triangle> titleValue))
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
					if(mapAICellData.trianglesToLink.TryGetValue(find, out var linkList))
					{
						linkTriangles = linkList;
					}
					else
					{
						linkTriangles = linkTriangleList.Where(item => item.IsThis(find.Index)).ToArray();
					}
					return linkTriangles.Length > 0;
				}
			}
			triangle = default;
			linkTriangles = null;
			return false;
		}
		public bool FindLinkTriangle(Triangle A, Triangle B, out LinkTriangle linkTriangles)
		{
			linkTriangles = default;
			foreach(var link in linkTriangleList)
			{
				if(link.IsThis(A.Index, B.Index))
				{
					linkTriangles = link;
					return true;
				}
			}
			return false;
		}

#if UNITY_EDITOR
		private EditorCoroutine editorCoroutine = null;
		[Button]
		private void UpdateOneFrame()
		{
			if(editorCoroutine != null)
			{
				EditorCoroutineUtility.StopCoroutine(editorCoroutine);
			}
			triangleList = new List<Triangle>();
			linkTriangleList = new List<LinkTriangle>();
			editorCoroutine = EditorCoroutineUtility.StartCoroutine(AsyncUpdate(), this);
		}

		public void OnDrawGizmos()
		{
			if(!IsOnDrawGizmos) return;
			if(triangleList == null) return;
			if(linkTriangleList == null) return;

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
					Vector3 center = triangle.Center();
					Gizmos.DrawSphere(center + offset, 0.5f);


					var thisLinkTriangle = linkTriangleList.Where(item => item.IsThis(triangle.Index));
					foreach(var link in thisLinkTriangle)
					{
						if(IsOnDrawGizmos_ShowOnlyPerfectLink)
						{
							if(link.IsPerfectLink)
							{
								Gizmos.color = Color.green;
								link.OnDrawGizmos(offset, 0.25f);
							}
						}
						else
						{
							if(link.IsPerfectLink)
							{
								Gizmos.color = Color.green;
							}
							else
							{
								//float lerp = (float)link.linkCount / 9f;
								//Gizmos.color = Color.Lerp(Color.red, Color.yellow, lerp);
								Gizmos.color = Color.yellow;
							}
							link.OnDrawGizmos(offset, 0.25f);
						}
					}
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
