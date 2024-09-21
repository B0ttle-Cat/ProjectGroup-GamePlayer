using System;

namespace BC.OdccBase
{
	public static class AbilityMath
	{
		[Serializable]
		public struct AbilityRange<T>
		{
			public T Start;
			public T Ended;
		};
		[Serializable]
		public struct AbilityValue<T>
		{
			public T Value;
		};
	}
}
