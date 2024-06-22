using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public class AbilityMinMaxCalculation : AbilityCalculationModule
	{
		[TitleGroup("Min Max Range")]
		[HorizontalGroup("Min Max Range/H")]
		[SerializeReference, HideLabel, InlineProperty]
		public NumberValue min, max;

		public override void Calculation(ref dynamic value)
		{
			if(min.Value < value)
			{
				value = min.Value;
			}
			if(max.Value > value)
			{
				value = max.Value;
			}
		}
	}
}
