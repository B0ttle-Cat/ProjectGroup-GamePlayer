namespace BC.OdccBase
{
	public class HPAbilityObject : AbilityObject
	{
		public override void BaseAbilityAwake()
		{
		}

		public override void BaseAbilityDestroy()
		{
		}

		public override void OnCalculation()
		{
			double value = Calculation(ThisAbility.value);
			ThisAbility.value = value;
		}
		protected override void LastFilter(ref double value)
		{
			if(value < 0d)
				value = 0d;
			if(value > 1000_0000_0000d)
				value = 1000_0000_0000d;
		}
	}
}
