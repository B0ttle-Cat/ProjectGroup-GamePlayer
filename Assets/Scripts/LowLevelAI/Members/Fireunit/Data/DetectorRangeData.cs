using System;

using BC.ODCC;

using RangeAttribute = UnityEngine.RangeAttribute;

namespace BC.LowLevelAI
{
	[Serializable]
	public class DetectorRangeData : DataObject
	{
		public float detectingRnage;
		public float minimumDetectingRnage;
		public float maximumDetectingRnage;
		[Range(0f,180f)]
		public float detectingYAngle;

		public DetectorRangeData()
		{
			detectingRnage = 15f;
			detectingYAngle=180f;

			minimumDetectingRnage = 5f;
			maximumDetectingRnage = 30f;
		}
	}
}
