using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

namespace BC.LowLevelAI
{
	public class MapWaypointComputer : ComponentBehaviour
	{
		NavMeshConnectComputer navMeshConnectComputer;
		MapAICellData mapAICellData;
#if UNITY_EDITOR
		[SerializeField]
		private bool IsOnDrawGizmos;
#endif
		[ShowInInspector, ReadOnly]
		public bool IsComputing { get; private set; } = true;

		OdccQueryCollector waypointCollector;
		MapWaypoint[] allWaypoints;

		public override void BaseAwake()
		{
			navMeshConnectComputer = ThisContainer.GetComponent<NavMeshConnectComputer>();
			mapAICellData = ThisContainer.GetData<MapAICellData>();

			waypointCollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery()
				.WithAll<MapAnchor, MapWaypoint>()
				.WithAny<MapWaypoint, StrategicPoint>()
				.Build());
		}
		public override void BaseEnable()
		{
			waypointCollector.CreateChangeListEvent(InitItems, UpdateItme)
				.CreateCallEvent(nameof(MapWaypointComputer))
				.Action(StartUpdate)
				.Foreach<MapAnchor, MapWaypoint, StrategicPoint>(WaypointConnectUpdate)
				.Foreach<MapAnchor, MapWaypoint, StrategicPoint>(StrategicPointUpdate)
				.Action(UpdateCloseWaypoint)
				.Action(EndedUpdate)
				.RunCallEvent();

			waypointCollector.DeleteChangeListEvent(UpdateItme)
				.DeleteCallEvent(nameof(MapWaypointComputer));
		}
		public override void BaseDisable()
		{
			waypointCollector.DeleteChangeListEvent(UpdateItme);
			waypointCollector.DeleteCallEvent(nameof(MapWaypointComputer));
		}
		private void InitItems(IEnumerable<ObjectBehaviour> enumerable)
		{
			allWaypoints = waypointCollector.GetQueryItems()
				.Select(item => item.ThisContainer.GetComponent<MapWaypoint>())
				.Where(item => item != null && item.isActiveAndEnabled).ToArray();
		}
		private void UpdateItme(ObjectBehaviour item, bool added)
		{
			if(item.ThisContainer.TryGetComponent(out MapWaypoint waypoint))
			{
				List<MapWaypoint> values = allWaypoints.ToList();
				if(added)
				{
					if(!values.Contains(waypoint))
					{
						values.Add(waypoint);
						allWaypoints = values.ToArray();
					}
				}
				else
				{
					if(values.Remove(waypoint))
					{
						allWaypoints = values.ToArray();
					}
				}
			}
		}

		private void StartUpdate()
		{
			IsComputing = true;
		}

		private void WaypointConnectUpdate(MapAnchor mapAnchor, MapWaypoint waypoint, StrategicPoint strategicPoint)
		{
			if(!mapAnchor.isActiveAndEnabled || !waypoint.isActiveAndEnabled)
			{
				return;
			}

			int length = allWaypoints.Length;
			List<MapWaypoint> asyncWaypointList = new List<MapWaypoint>();
			waypoint.CheckConnectStart();
			for(int i = 0 ; i < length ; i++)
			{
				var targetWaypoint = allWaypoints[i];
				if(targetWaypoint  == waypoint) continue;
				if(targetWaypoint.breakPass ||!targetWaypoint.isActiveAndEnabled) continue;

				waypoint.CheckConnectUpdate(targetWaypoint, asyncWaypointList);
			}
			waypoint.CheckConnectEnded(asyncWaypointList);
		}
		private void StrategicPointUpdate(MapAnchor mapAnchor, MapWaypoint waypoint, StrategicPoint strategicPoint)
		{
			if(!mapAnchor.isActiveAndEnabled || !waypoint.isActiveAndEnabled)
			{
				return;
			}

			if(strategicPoint != null) strategicPoint.StrategicPointUpdate();
		}
		private void UpdateCloseWaypoint()
		{
			//var list = navMeshConnectComputer.triangleList;
			//
			//int Count = list.Count;
			//for(int i = 0 ; i < Count ; i++)
			//{
			//	var triangle = list[i];
			//	Vector3 center = triangle.Center();
			//	MapWaypoint closeWaypoint = FindCloseWaypoint(center);
			//
			//
			//}
			//int Length = allWaypoints.Length;
			//for(int i = 0 ; i < Length ; i++)
			//{
			//	MapWaypoint waypoint = allWaypoints[i];
			//	Vector3 position = waypoint.OnNavMeshPosition;
			//	Vector3Int cellIndex = mapAICellData.GetCellIndex(position);
			//	if(mapAICellData.IndexToTriangles(cellIndex, out var triangles))
			//	{
			//		int Count = triangles.Count;
			//		for(int ii = 0 ; ii < Count ; ii++)
			//		{
			//			var triangle = triangles[ii];
			//			Vector3 center = triangle.Center();
			//
			//			Vector3 closestPoint = waypoint.ClosestPoint(position, out bool IsInside);
			//			if(IsInside)
			//			{
			//				mapAICellData.closeWaypoint.Add()
			//				continue;
			//			}
			//		}
			//	}
			//}

			//	mapAICellData.IndexItemList
		}
		private void EndedUpdate()
		{
			IsComputing = false;
		}

		public MapWaypoint FindCloseWaypoint(Vector3 position)
		{
			if(NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
			{
				position = hit.position;
			}

			int length = allWaypoints.Length;
			MapWaypoint waypoint = null;
			int index = 0;
			float minDistance = 0f;
			for(int i = 0 ; i < length ; i++)
			{
				waypoint = allWaypoints[i];
				Vector3 closestPoint = waypoint.ClosestPoint(position, out float distance);

				if(i == 0 || distance > minDistance)
				{
					index = i;
					distance = minDistance;
					if(distance == 0f) break;
				}
			}
			return allWaypoints[index];
		}

		public MapWaypoint SelectAnchorIndex(int selectIndex)
		{
			int length = allWaypoints.Length;
			for(int i = 0 ; i < length ; i++)
			{
				MapWaypoint waypoint = allWaypoints[i];
				if(allWaypoints[i].ThisContainer.TryGetData<MapAnchorData>(out var data))
				{
					if(data.anchorIndex == selectIndex)
					{
						return waypoint;
					}
				}
			}
			return null;
		}

#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if(!IsOnDrawGizmos) return;
			if(allWaypoints == null) return;

			int Length = allWaypoints.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				var waypoint = allWaypoints[i];
				if(waypoint == null) continue;

				Vector3 positionA = waypoint.OnNavMeshPosition + Vector3.up * 4f;
				int connectLength = waypoint.nextWaypointList.Length;
				Gizmos.color = Color.white;
				for(int ii = 0 ; ii <  connectLength ; ii++)
				{
					var other = waypoint.nextWaypointList[ii];
					Vector3 positionB = other.OnNavMeshPosition+ Vector3.up * 4f;
					Gizmos.DrawLine(positionA, positionB);
				}

				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(positionA, .5f);
			}
		}
#endif
	}
}
