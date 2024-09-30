using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

namespace BC.Map
{
	public class MapPathPointComputer : ComponentBehaviour
	{
		MapCellData mapCellData;
		[ShowInInspector, ReadOnly]
		private bool isComputing = false;
		public bool IsComputing { get => isComputing; private set => isComputing = value; }

		//OdccQueryCollector pathPointCollector;
		[ShowInInspector, ReadOnly]
		private MapAnchor[] allMapAnchor;
		private TupleDictionary<MapAnchor, MapAnchorData, MapPathPoint> mapAnchorLink;

		Coroutine asyncUpdate;
		public float tiemForCalculationCount = 1/10;
		private DateTime previousTime;
		public bool IsAsyncUpdate =>
#if UNITY_EDITOR
			editorCoroutine != null ||
#endif
			asyncUpdate != null;

		public bool IsCompleteUpdate => !IsAsyncUpdate && AllMapAnchor != null && AllMapAnchor.Length > 0;

		public MapAnchor[] AllMapAnchor {
			get => allMapAnchor;
			set {
				allMapAnchor=value;

				if(allMapAnchor == null)
				{
					mapAnchorLink = null;
				}
				else if(allMapAnchor.Length == 0)
				{
					mapAnchorLink = new TupleDictionary<MapAnchor, MapAnchorData, MapPathPoint>();
				}
				else
				{
					mapAnchorLink = new TupleDictionary<MapAnchor, MapAnchorData, MapPathPoint>();
					foreach(var item in allMapAnchor)
					{
						mapAnchorLink.Add(item, new TupleItem<MapAnchorData, MapPathPoint>(item.ThisContainer.GetData<MapAnchorData>(), item.ThisContainer.GetComponent<MapPathPoint>()));
					}
				}
			}
		}
		protected override void Disposing()
		{
			base.Disposing();

			asyncUpdate = null;
			mapCellData = null;
			allMapAnchor = null;
			mapAnchorLink = null;
		}
		public override void BaseAwake()
		{
			IsComputing = false;
			AllMapAnchor = new MapAnchor[0];
			//allMapAnchorData = new Dictionary<MapAnchor, MapAnchorData>();
			//allMapPathPoint = new Dictionary<MapAnchor, MapPathPoint>();
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
			AllMapAnchor = new MapAnchor[0];
			if(IsAsyncUpdate)
			{
				StopCoroutine(asyncUpdate);
			}
			asyncUpdate = StartCoroutine(AsyncUpdate());
		}
		public override void BaseDisable()
		{
			AllMapAnchor = new MapAnchor[0];
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
				.Where(item => item != null);
			AllMapAnchor = _allPathPoints.ToArray();

			yield return null;

			foreach(var item in mapAnchorLink)
			{
				PathPointSetNavMeshPosition(item.Value.Item2);
			}
			foreach(var item in mapAnchorLink)
			{
				yield return PathPointConnectNeighbor(item.Value.Item2);
			}
			asyncUpdate = null;
			yield break;
		}


		private void PathPointSetNavMeshPosition(MapPathPoint pathPoint)
		{
			if(pathPoint == null) return;
			pathPoint.SetNavMeshPosition();
		}
		private IEnumerator PathPointConnectNeighbor(MapPathPoint pathPoint)
		{
			if(pathPoint == null || !pathPoint.isActiveAndEnabled)
			{
				yield break;
			}

			List<MapPathPoint> asyncPathPointList = new List<MapPathPoint>();

			foreach(var item in mapAnchorLink)
			{
				var targetPathPoint = item.Value.Item2;
				if(targetPathPoint == null || targetPathPoint == pathPoint) continue;
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
		public bool TrySelectAnchorIndex(int selectIndex, out MapAnchor mapAnchor)
		{
			mapAnchor = null;
			int length = AllMapAnchor.Length;
			for(int i = 0 ; i < length ; i++)
			{
				MapAnchor pathpoint = AllMapAnchor[i];
				if(pathpoint.ThisContainer.TryGetData<MapAnchorData>(out var data))
				{
					if(data.anchorIndex == selectIndex)
					{
						mapAnchor = pathpoint;
						break;
					}
				}
			}
			return mapAnchor != null;
		}
		public bool TrySelectPathPointIndex(int selectIndex, out MapPathPoint mapPathPoint)
		{
			mapPathPoint = null;
			int length = AllMapAnchor.Length;
			foreach(var item in mapAnchorLink)
			{
				var data = item.Value.Item1;
				if(data != null && data.anchorIndex == selectIndex)
				{
					var pathpoint = item.Value.Item2;
					if(pathpoint != null)
					{
						mapPathPoint = pathpoint;
						break;
					}
				}
			}
			return mapPathPoint != null;
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

			if(mapCellData.trianglesClosedPoint.TryGetValue(triangles[inTriangles], out var _mapPathPoint))
			{
				mapPathPoint = (MapPathPoint)_mapPathPoint;
			}
			return mapPathPoint != null;
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
			if(AllMapAnchor == null || mapAnchorLink == null) return;

			foreach(var item in mapAnchorLink)
			{
				var pathpoint = item.Value.Item2;
				if(pathpoint == null) continue;
				Vector3 positionA = pathpoint.closeNavMeshPosition + Vector3.up * 4f;
				int connectLength = pathpoint.nextPathPointList.Length;
				Gizmos.color = Color.white;
				for(int i = 0 ; i <  connectLength ; i++)
				{
					var other = pathpoint.nextPathPointList[i];
					Vector3 positionB = other.closeNavMeshPosition+ Vector3.up * 4f;
					Gizmos.DrawLine(positionA, positionB);
				}
			}
			foreach(var item in mapAnchorLink)
			{
				var pathpoint = item.Value.Item2;
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
					if(TrySelectPathPointIndex(startPointId, out var start))
					{
						TrySelectPathPointIndex(endedPointId, out var ended);
						if(start.CalculatePath(ended, out var node))
						{
							debugMapPathNode = node as MapPathNode;
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
