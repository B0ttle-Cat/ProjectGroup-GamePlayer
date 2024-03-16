using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using UnityEngine;
using UnityEngine.AI;

namespace BC.LowLevelAI
{
	public class MapWaypoint : ComponentBehaviour
	{
		private MapAnchor mapAnchor;

		public bool breakPass;
		[SerializeField]
		private float defaultWayCost = 1f;
		public Vector3 OnNavMeshPosition;
		public MapWaypoint[] nextWaypointList;

		private float variableWayCost = 0f;

		public float WayCost => breakPass ? float.PositiveInfinity : defaultWayCost + variableWayCost;

		public override void BaseAwake()
		{
			base.BaseAwake();
			mapAnchor = ThisObject as MapAnchor;
		}

		public override void BaseEnable()
		{
			nextWaypointList = new MapWaypoint[0];

			base.BaseEnable();
			OnNavMeshPosition = ThisTransform.position;
			if(NavMesh.SamplePosition(OnNavMeshPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
			{
				OnNavMeshPosition = hit.position;
			}
		}


		internal void CheckConnectStart()
		{
			nextWaypointList = new MapWaypoint[0];
		}
		internal void CheckConnectUpdate(MapWaypoint targetWaypoint, List<MapWaypoint> asyncWaypointList)
		{
			Vector3 a = OnNavMeshPosition + Vector3.up;
			Vector3 b = targetWaypoint.OnNavMeshPosition + Vector3.up;

			Vector3 diraction = b - a;
			float magnitude = diraction.magnitude;
			diraction = diraction.normalized;
			Ray ray = new Ray(a, diraction);

			//List<MapWaypoint> asyncWaypointList = nextWaypointList.ToList();
			if(Physics.Raycast(ray, out var hit, magnitude, TagAndLayer.GetHitLayerMask(TagAndLayer.MapAnchor), QueryTriggerInteraction.Collide))
			{
				MapWaypoint mapWaypoint = hit.collider.gameObject.GetComponentInParent<MapWaypoint>();

				if(mapWaypoint is not null)
				{
					if(mapWaypoint == targetWaypoint)
					{
						asyncWaypointList.Add(targetWaypoint);
					}
				}
			}
		}

		internal void CheckConnectEnded(List<MapWaypoint> asyncWaypointList)
		{
			nextWaypointList = asyncWaypointList.ToArray();
		}


		public bool InSidePosition(Vector3 position)
		{
			return mapAnchor.InSidePosition(position);
		}
		public Vector3 ClosestPoint(Vector3 position, out float distance)
		{
			return mapAnchor.ClosestPoint(position, out distance);
		}
	}
}
