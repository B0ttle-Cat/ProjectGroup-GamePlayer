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
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<MapAnchorData>(out var anchorData))
			{
				anchorData = ThisContainer.AddData<MapAnchorData>();
			}
			anchorData.anchorIndex = ThisTransform.GetSiblingIndex();
		}


		public override void BaseAwake()
		{
			base.BaseAwake();
			var trigger = AnchorTrigger;
			int Length = trigger.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				trigger[i].isTrigger = true;
			}
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

		internal Vector3 ClosestPoint(Vector3 position, out float distance)
		{
			var list = AnchorTrigger;
			int length = list.Length;
			Vector3 minClose = Vector3.zero;
			distance = 0;
			for(int i = 0 ; i < length ; i++)
			{
				var trigger = list[i];

				Vector3 closestPoint = trigger.ClosestPoint(position);
				float checkDistance = (position - closestPoint).sqrMagnitude;

				if(i == 0 || distance > checkDistance)
				{
					distance = checkDistance;
					minClose = closestPoint;
				}
			}
			distance = Vector3.Distance(position, minClose);
			return minClose;
		}
	}
}
