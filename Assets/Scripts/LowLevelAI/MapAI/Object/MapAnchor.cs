using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class MapAnchor : ObjectBehaviour
	{
		[SerializeField]
		private Collider[] mapAnchorTrigger;

		public Collider[] AnchorTrigger {
			get
			{
				if(mapAnchorTrigger == null)
				{
					mapAnchorTrigger = GetComponentsInChildren<Collider>();
				}
				return mapAnchorTrigger;
			}
		}
		public override void BaseReset()
		{
			base.BaseReset();
			mapAnchorTrigger = GetComponentsInChildren<Collider>();
		}
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<MapAnchorData>(out var anchorData))
			{
				anchorData = ThisContainer.AddData<MapAnchorData>();
			}
			anchorData.anchorIndex = ThisTransform.GetSiblingIndex();
			if(ThisContainer.TryGetComponent<StrategicPoint>(out _))
			{
				gameObject.name = $"{anchorData.anchorIndex:00}\t: StrategicPoint";
			}
			else if(ThisContainer.TryGetComponent<MapPathPoint>(out _))
			{
				gameObject.name = $"{anchorData.anchorIndex:00}\t: PathPoint";
			}
			else
			{
				gameObject.name = $"{anchorData.anchorIndex:00}\t: NonePoint";
			}
			mapAnchorTrigger = GetComponentsInChildren<Collider>();
		}


		public override void BaseAwake()
		{
			base.BaseAwake();
			mapAnchorTrigger = GetComponentsInChildren<Collider>();

			var trigger = AnchorTrigger;
			int Length = trigger.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				trigger[i].isTrigger = true;
			}
		}

		public Vector3 ThisPosition()
		{
			return ThisTransform.position;
		}
		public bool InSidePosition(Vector3 position)
		{
			var list = AnchorTrigger;
			int length = list.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var trigger = list[i];
				Vector3 inSidePosition = trigger.ClosestPoint(position);
				if(position == inSidePosition)
				{
					return true;
				}
			}

			return false;
		}

		public Vector3 ClosestPoint(Vector3 position, out float distance)
		{
			var list = AnchorTrigger;
			int length = list.Length;
			Vector3 minClose = ThisPosition();
			distance = 0;
			for(int i = 0 ; i < length ; i++)
			{
				var trigger = list[i];

				Vector3 closestPoint = trigger.ClosestPoint(position);
				float checkDistance = Vector3.Distance(position, closestPoint);

				if(i == 0 || distance > checkDistance)
				{
					distance = checkDistance;
					minClose = closestPoint;
				}
			}
			return minClose;
		}


	}
}
