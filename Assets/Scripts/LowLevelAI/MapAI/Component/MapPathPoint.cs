using System.Collections.Generic;

using BC.Base;
using BC.ODCC;

using UnityEngine;
using UnityEngine.AI;

namespace BC.LowLevelAI
{
	public class MapPathPoint : ComponentBehaviour
	{
		private MapAnchor mapAnchor;

		public bool breakPass;
		[SerializeField]
		private float defaultWayCost = 1f;
		public Vector3 OnNavMeshPosition;
		public MapPathPoint[] nextPathpointList;

		private float variableWayCost = 0f;

		public float WayCost => breakPass ? float.PositiveInfinity : defaultWayCost + variableWayCost;
		public override void BaseValidate()
		{
			base.BaseValidate();
			mapAnchor = ThisObject as MapAnchor;
		}

		public override void BaseAwake()
		{
			base.BaseAwake();
			mapAnchor = ThisObject as MapAnchor;
		}

		public override void BaseEnable()
		{
			nextPathpointList = new MapPathPoint[0];

			base.BaseEnable();
			OnNavMeshPosition = ThisTransform.position;
			if(NavMesh.SamplePosition(OnNavMeshPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
			{
				OnNavMeshPosition = hit.position;
			}
		}


		internal void CheckConnectStart()
		{
			nextPathpointList = new MapPathPoint[0];
		}
		internal void CheckConnectUpdate(MapPathPoint targetPathpoint, List<MapPathPoint> asyncPathpointList)
		{
			Vector3 a = OnNavMeshPosition + Vector3.up;
			Vector3 b = targetPathpoint.OnNavMeshPosition + Vector3.up;

			Vector3 diraction = b - a;
			float magnitude = diraction.magnitude;
			diraction = diraction.normalized;
			Ray ray = new Ray(a, diraction);

			//List<MapPathPoint> asyncPathpointList = nextPathpointList.ToList();
			if(Physics.Raycast(ray, out var hit, magnitude, TagAndLayer.GetHitLayerMask(TagAndLayer.MapAnchor), QueryTriggerInteraction.Collide))
			{
				MapPathPoint mapPathpoint = hit.collider.gameObject.GetComponentInParent<MapPathPoint>();

				if(mapPathpoint is not null)
				{
					if(mapPathpoint == targetPathpoint)
					{
						asyncPathpointList.Add(targetPathpoint);
					}
				}
			}
		}

		internal void CheckConnectEnded(List<MapPathPoint> asyncPathpointList)
		{
			nextPathpointList = asyncPathpointList.ToArray();
		}

		public Vector3 ThisPosition()
		{
			return mapAnchor.ThisPosition();
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
