using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

namespace BC.LowLevelAI
{
	public class MapPathPointComputer : ComponentBehaviour
	{
		MapCellData mapCellData;
		[ShowInInspector, ReadOnly]
		private bool isComputing = false;
		public bool IsComputing { get => isComputing; private set => isComputing = value; }

		//OdccQueryCollector pathPointCollector;
		[ShowInInspector, ReadOnly]
		private MapPathPoint[] allPathPoints;

		Coroutine asyncUpdate;
		public float tiemForCalculationCount = 1/10;
		private DateTime previousTime;
		public bool IsAsyncUpdate =>
#if UNITY_EDITOR
			editorCoroutine != null ||
#endif
			asyncUpdate != null;

		public override void BaseAwake()
		{
			IsComputing = false;
			allPathPoints = new MapPathPoint[0];
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
#if UNITY_EDITOR
			editorCoroutine = null;
#endif
			if(IsAsyncUpdate)
			{
				StopCoroutine(asyncUpdate);
			}
			asyncUpdate = StartCoroutine(AsyncUpdate());
		}
		public override void BaseDisable()
		{
			allPathPoints = new MapPathPoint[0];
			if(IsAsyncUpdate)
			{
				StopCoroutine(asyncUpdate);
				asyncUpdate = null;
			}
		}

		public IEnumerator AsyncUpdate()
		{
			yield return null;
			while(mapCellData == null)
			{
				mapCellData = ThisContainer.GetData<MapCellData>();
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

			var _allPathPoints = ThisContainer.GetChildAllObject<MapAnchor>()
				.Select(item => item.ThisContainer.GetComponent<MapPathPoint>())
				.Where(item => item != null);
			allPathPoints = _allPathPoints.ToArray();

			yield return null;

			foreach(var item in _allPathPoints)
			{
				PathPointSetNavMeshPosition(item);
			}
			foreach(var item in _allPathPoints)
			{
				yield return PathPointConnectNeighbor(item);
			}
			asyncUpdate = null;
			yield break;
		}


		private void PathPointSetNavMeshPosition(MapPathPoint pathPoint)
		{
			pathPoint.SetNavMeshPosition();
		}
		private IEnumerator PathPointConnectNeighbor(MapPathPoint pathPoint)
		{
			if(!pathPoint.isActiveAndEnabled)
			{
				yield break;
			}

			List<MapPathPoint> asyncPathPointList = new List<MapPathPoint>();
			int length = allPathPoints.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var targetPathPoint = allPathPoints[i];
				if(targetPathPoint == pathPoint) continue;
				if(targetPathPoint.IsBrakePath ||!targetPathPoint.isActiveAndEnabled) continue;

				pathPoint.ConnectingNeighbor(targetPathPoint, asyncPathPointList);

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

			pathPoint.ConnectNeighborEnded(asyncPathPointList);
		}

		public MapPathPoint SelectAnchorIndex(int selectIndex)
		{
			int length = allPathPoints.Length;
			for(int i = 0 ; i < length ; i++)
			{
				MapPathPoint pathpoint = allPathPoints[i];
				if(pathpoint.ThisContainer.TryGetData<MapAnchorData>(out var data))
				{
					if(data.anchorIndex == selectIndex)
					{
						return pathpoint;
					}
				}
			}
			return null;
		}
		public bool TryGetClosedPathPoint(Vector3 position, out MapPathPoint mapPathPoint)
		{
			if(NavMesh.SamplePosition(position, out var hit, mapCellData.navMeshSerchRange, NavMesh.AllAreas))
			{
				position = hit.position;
			}

			mapPathPoint = null;
			Vector3Int index = mapCellData.VectorToIndex(position);
			bool findTriangles = mapCellData.IndexToTriangles(index, out var triangles);
			if(!findTriangles) return false;

			int length = triangles.Count;
			int inTriangles = -1;
			for(int i = 0 ; i < length ; i++)
			{
				if(triangles[i].IsPointInTriangle(position))
				{
					inTriangles = i;
					break;
				}
			}
			if(inTriangles < 0) return false;

			bool isFind = mapCellData.trianglesClosedPoint.TryGetValue(triangles[inTriangles], out mapPathPoint);

			return isFind && mapPathPoint != null;
		}

		//linkRayTriangleList = new List<LinkRayTriangle>();
#if UNITY_EDITOR
		private Unity.EditorCoroutines.Editor.EditorCoroutine editorCoroutine = null;
		[Button]
		private void UpdateInEditor()
		{
			if(editorCoroutine != null)
			{
				Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StopCoroutine(editorCoroutine);
			}
			editorCoroutine = Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(AsyncUpdate(), this);
		}

		[Header("OnDrawGizmos")]
		[SerializeField]
		private bool IsOnDrawGizmos;
		[SerializeField, ShowIf("@IsOnDrawGizmos")]
		private int startPointId = -1;
		[SerializeField, ShowIf("@IsOnDrawGizmos")]
		private int endedPointId= -1;
		[SerializeField, ShowIf("@IsOnDrawGizmos")]
		private bool debugCalculatePath;
		private MapPathNode debugMapPathNode;
		public void OnDrawGizmos()
		{
			if(!IsOnDrawGizmos) return;
			if(allPathPoints == null) return;

			int Length = allPathPoints.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				var pathpoint = allPathPoints[i];
				if(pathpoint == null) continue;

				Vector3 positionA = pathpoint.closeNavMeshPosition + Vector3.up * 4f;
				int connectLength = pathpoint.nextPathPointList.Length;
				Gizmos.color = Color.white;
				for(int ii = 0 ; ii <  connectLength ; ii++)
				{
					var other = pathpoint.nextPathPointList[ii];
					Vector3 positionB = other.closeNavMeshPosition+ Vector3.up * 4f;
					Gizmos.DrawLine(positionA, positionB);
				}
			}
			for(int i = 0 ; i < Length ; i++)
			{
				var pathpoint = allPathPoints[i];
				if(pathpoint == null) continue;
				Vector3 positionA = pathpoint.closeNavMeshPosition + Vector3.up * 4f;
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(positionA, .5f);
			}
			if(debugCalculatePath)
			{
				debugCalculatePath = false;
				debugMapPathNode = null;
				if(startPointId >= 0 && endedPointId >= 0)
				{
					var start = SelectAnchorIndex(startPointId);
					var ended = SelectAnchorIndex(endedPointId);
					if(start != null)
					{
						if(start.CalculatePath(ended, out MapPathNode node))
						{
							debugMapPathNode = node;
						}
						else
						{
							debugMapPathNode = null;
						}
					}
				}
			}
			if(debugMapPathNode != null)
			{
				Gizmos.color = Color.red;
				for(int i = 0 ; i < 25 ; i++)
				{
					debugMapPathNode.OnDrawGizmos(Vector3.up * (4f + 0.04f * i));
				}
			}
		}
#endif
	}
}
