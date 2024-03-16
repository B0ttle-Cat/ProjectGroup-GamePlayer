using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class DetectorRayData : DataObject
	{
		[ValueDropdown("@TagAndLayer.LayerIndexList")]
		public LayerMask rayHitLayer;
	}
}
