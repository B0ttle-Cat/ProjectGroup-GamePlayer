using System;

namespace BC.OdccBase
{
	public static class AbilityMath
	{
		[Serializable]
		public struct ValueRange
		{
			public float Start;
			public float Ended;
			public float Min;
			public float Max;
		};
		[Serializable]
		public struct ValueMinMax
		{
			public float Value;
			public float Min;
			public float Max;
		};
	}
}
