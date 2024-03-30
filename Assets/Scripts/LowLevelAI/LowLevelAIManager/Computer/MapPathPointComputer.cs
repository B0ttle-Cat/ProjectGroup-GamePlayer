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
		public bool IsComputing { get; private set; } = true;

		OdccQueryCollector pathPointCollector;
		MapPathPoint[] allPathPoints;

		public override void BaseAwake()
		{
			mapCellData = ThisContainer.GetData<MapCellData>();

			pathPointCollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery()
				.WithAll<MapAnchor, MapPathPoint>()
				.Build());
		}
		public override void BaseEnable()
		{
			pathPointCollector.CreateChangeListEvent(InitItems, UpdateItme)
				.CreateCallEvent(nameof(MapPathPointComputer))
				.Action(StartUpdate)
				.Foreach<MapAnchor, MapPathPoint>(PathPointConnectUpdate)
				//.Foreach<MapAnchor, MapPathPoint>(PathPointUpdate)
				.Action(EndedUpdate)
				.RunCallEvent();

			pathPointCollector.DeleteChangeListEvent(UpdateItme)
				.DeleteCallEvent(nameof(MapPathPointComputer));
		}
		public override void BaseDisable()
		{
			pathPointCollector.DeleteChangeListEvent(UpdateItme);
			pathPointCollector.DeleteCallEvent(nameof(MapPathPointComputer));
		}
		private void InitItems(IEnumerable<ObjectBehaviour> enumerable)
		{
			allPathPoints = pathPointCollector.GetQueryItems()
				.Select(item => item.ThisContainer.GetComponent<MapPathPoint>())
				.Where(item => item != null && item.isActiveAndEnabled).ToArray();
		}
		private void UpdateItme(ObjectBehaviour item, bool added)
		{
			if(item.ThisContainer.TryGetComponent(out MapPathPoint pathpoint))
			{
				List<MapPathPoint> values = allPathPoints.ToList();
				if(added)
				{
					if(!values.Contains(pathpoint))
					{
						values.Add(pathpoint);
						allPathPoints = values.ToArray();
					}
				}
				else
				{
					if(values.Remove(pathpoint))
					{
						allPathPoints = values.ToArray();
					}
				}
			}
		}

		private void StartUpdate()
		{
			IsComputing = true;
		}

		private void PathPointConnectUpdate(MapAnchor mapAnchor, MapPathPoint pathPoint)
		{
			if(!mapAnchor.isActiveAndEnabled || !pathPoint.isActiveAndEnabled)
			{
				return;
			}

			int length = allPathPoints.Length;
			List<MapPathPoint> asyncPathPointList = new List<MapPathPoint>();
			pathPoint.CheckConnectStart();
			for(int i = 0 ; i < length ; i++)
			{
				var targetPathPoint = allPathPoints[i];
				if(targetPathPoint  == pathPoint) continue;
				if(targetPathPoint.IsBrakePath ||!targetPathPoint.isActiveAndEnabled) continue;

				pathPoint.CheckConnectUpdate(targetPathPoint, asyncPathPointList);
			}
			pathPoint.CheckConnectEnded(asyncPathPointList);
		}
		//private void StrategicPointUpdate(MapAnchor mapAnchor, MapPathPoint pathPoint, StrategicPoint strategicPoint)
		//{
		//	if(!mapAnchor.isActiveAndEnabled || !pathPoint.isActiveAndEnabled)
		//	{
		//		return;
		//	}
		//
		//	//if(strategicPoint != null) strategicPoint.StrategicPointUpdate();
		//}

		private void EndedUpdate()
		{
			IsComputing = false;
		}

		public MapPathPoint FindClosePathPoint(Vector3 position)
		{
			if(NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
			{
				position = hit.position;
			}

			int length = allPathPoints.Length;
			MapPathPoint pathpoint = null;
			int index = 0;
			float minDistance = 0f;
			for(int i = 0 ; i < length ; i++)
			{
				pathpoint = allPathPoints[i];
				Vector3 closestPoint = pathpoint.ClosestPoint(position, out float distance);

				if(i == 0 || distance > minDistance)
				{
					index = i;
					distance = minDistance;
					if(distance == 0f) break;
				}
			}
			return allPathPoints[index];
		}
		public MapPathPoint SelectAnchorIndex(int selectIndex)
		{
			int length = allPathPoints.Length;
			for(int i = 0 ; i < length ; i++)
			{
				MapPathPoint pathpoint = allPathPoints[i];
				if(allPathPoints[i].ThisContainer.TryGetData<MapAnchorData>(out var data))
				{
					if(data.anchorIndex == selectIndex)
					{
						return pathpoint;
					}
				}
			}
			return null;
		}

#if UNITY_EDITOR
		[Header("OnDrawGizmos")]
		[SerializeField]
		private bool IsOnDrawGizmos;
		[SerializeField] private int startPointId;
		[SerializeField] private int endedPointId;

		public void OnDrawGizmos()
		{
			if(!IsOnDrawGizmos) return;
			if(allPathPoints == null) return;

			int Length = allPathPoints.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				var pathpoint = allPathPoints[i];
				if(pathpoint == null) continue;

				Vector3 positionA = pathpoint.OnNavMeshPosition + Vector3.up * 4f;
				int connectLength = pathpoint.nextPathPointList.Length;
				Gizmos.color = Color.white;
				for(int ii = 0 ; ii <  connectLength ; ii++)
				{
					var other = pathpoint.nextPathPointList[ii];
					Vector3 positionB = other.OnNavMeshPosition+ Vector3.up * 4f;
					Gizmos.DrawLine(positionA, positionB);
				}

				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(positionA, .5f);
			}

			if(startPointId >= 0 && endedPointId >= 0)
			{

			}
		}
#endif
	}
}
