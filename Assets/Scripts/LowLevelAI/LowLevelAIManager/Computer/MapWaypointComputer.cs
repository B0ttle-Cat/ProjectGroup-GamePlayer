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
#if UNITY_EDITOR
		[SerializeField]
		private bool IsOnDrawGizmos;
#endif
		[ShowInInspector, ReadOnly]
		public bool IsComputing { get; private set; } = true;

		OdccQueryCollector pathpointCollector;
		MapPathPoint[] allPathpoints;

		public override void BaseAwake()
		{
			mapCellData = ThisContainer.GetData<MapCellData>();

			pathpointCollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery()
				.WithAll<MapAnchor, MapPathPoint>()
				.WithAny<MapPathPoint, StrategicPoint>()
				.Build());
		}
		public override void BaseEnable()
		{
			pathpointCollector.CreateChangeListEvent(InitItems, UpdateItme)
				.CreateCallEvent(nameof(MapPathPointComputer))
				.Action(StartUpdate)
				.Foreach<MapAnchor, MapPathPoint, StrategicPoint>(PathpointConnectUpdate)
				.Foreach<MapAnchor, MapPathPoint, StrategicPoint>(StrategicPointUpdate)
				.Action(EndedUpdate)
				.RunCallEvent();

			pathpointCollector.DeleteChangeListEvent(UpdateItme)
				.DeleteCallEvent(nameof(MapPathPointComputer));
		}
		public override void BaseDisable()
		{
			pathpointCollector.DeleteChangeListEvent(UpdateItme);
			pathpointCollector.DeleteCallEvent(nameof(MapPathPointComputer));
		}
		private void InitItems(IEnumerable<ObjectBehaviour> enumerable)
		{
			allPathpoints = pathpointCollector.GetQueryItems()
				.Select(item => item.ThisContainer.GetComponent<MapPathPoint>())
				.Where(item => item != null && item.isActiveAndEnabled).ToArray();
		}
		private void UpdateItme(ObjectBehaviour item, bool added)
		{
			if(item.ThisContainer.TryGetComponent(out MapPathPoint pathpoint))
			{
				List<MapPathPoint> values = allPathpoints.ToList();
				if(added)
				{
					if(!values.Contains(pathpoint))
					{
						values.Add(pathpoint);
						allPathpoints = values.ToArray();
					}
				}
				else
				{
					if(values.Remove(pathpoint))
					{
						allPathpoints = values.ToArray();
					}
				}
			}
		}

		private void StartUpdate()
		{
			IsComputing = true;
		}

		private void PathpointConnectUpdate(MapAnchor mapAnchor, MapPathPoint pathpoint, StrategicPoint strategicPoint)
		{
			if(!mapAnchor.isActiveAndEnabled || !pathpoint.isActiveAndEnabled)
			{
				return;
			}

			int length = allPathpoints.Length;
			List<MapPathPoint> asyncPathpointList = new List<MapPathPoint>();
			pathpoint.CheckConnectStart();
			for(int i = 0 ; i < length ; i++)
			{
				var targetPathpoint = allPathpoints[i];
				if(targetPathpoint  == pathpoint) continue;
				if(targetPathpoint.breakPass ||!targetPathpoint.isActiveAndEnabled) continue;

				pathpoint.CheckConnectUpdate(targetPathpoint, asyncPathpointList);
			}
			pathpoint.CheckConnectEnded(asyncPathpointList);
		}
		private void StrategicPointUpdate(MapAnchor mapAnchor, MapPathPoint pathpoint, StrategicPoint strategicPoint)
		{
			if(!mapAnchor.isActiveAndEnabled || !pathpoint.isActiveAndEnabled)
			{
				return;
			}

			if(strategicPoint != null) strategicPoint.StrategicPointUpdate();
		}

		private void EndedUpdate()
		{
			IsComputing = false;
		}

		public MapPathPoint FindClosePathpoint(Vector3 position)
		{
			if(NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
			{
				position = hit.position;
			}

			int length = allPathpoints.Length;
			MapPathPoint pathpoint = null;
			int index = 0;
			float minDistance = 0f;
			for(int i = 0 ; i < length ; i++)
			{
				pathpoint = allPathpoints[i];
				Vector3 closestPoint = pathpoint.ClosestPoint(position, out float distance);

				if(i == 0 || distance > minDistance)
				{
					index = i;
					distance = minDistance;
					if(distance == 0f) break;
				}
			}
			return allPathpoints[index];
		}

		public MapPathPoint SelectAnchorIndex(int selectIndex)
		{
			int length = allPathpoints.Length;
			for(int i = 0 ; i < length ; i++)
			{
				MapPathPoint pathpoint = allPathpoints[i];
				if(allPathpoints[i].ThisContainer.TryGetData<MapAnchorData>(out var data))
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
		public void OnDrawGizmos()
		{
			if(!IsOnDrawGizmos) return;
			if(allPathpoints == null) return;

			int Length = allPathpoints.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				var pathpoint = allPathpoints[i];
				if(pathpoint == null) continue;

				Vector3 positionA = pathpoint.OnNavMeshPosition + Vector3.up * 4f;
				int connectLength = pathpoint.nextPathpointList.Length;
				Gizmos.color = Color.white;
				for(int ii = 0 ; ii <  connectLength ; ii++)
				{
					var other = pathpoint.nextPathpointList[ii];
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
