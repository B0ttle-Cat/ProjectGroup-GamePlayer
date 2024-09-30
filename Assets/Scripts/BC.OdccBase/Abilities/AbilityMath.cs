using System;

using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	public static class AbilityMath
	{
		[Serializable, InlineProperty]
		public struct AbilityRange<T>
		{
			[HideLabel]
			[HorizontalGroup]
			public T Start;
			[HideLabel]
			[HorizontalGroup]
			public T Ended;
		};
		[Serializable, InlineProperty]
		public struct AbilityValue<T>
		{
			[HideLabel]
			public T Value;
		};
	}
}
