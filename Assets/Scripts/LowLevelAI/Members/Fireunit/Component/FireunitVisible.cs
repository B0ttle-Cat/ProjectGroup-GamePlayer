using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitVisible : ComponentBehaviour, IUnitTarget
	{
		public float TargetingRadius { get; set; }
		public Vector3 TargetingPosition { get; set; }

		public FireunitData VisibleData => ThisContainer.GetData<FireunitData>();

		public float GetVisibleValue()
		{
			return 0;
		}

	}
}
