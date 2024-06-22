namespace BC.OdccBase
{
	public class HPAbilityObject : AbilityObject<LongNumber, long>
	{
		public override void BaseAbilityAwake()
		{
		}

		public override void BaseAbilityDestroy()
		{
		}

		public override long OnCalculation()
		{
			var value = Calculation(ThisValue.value);
			return value;
		}
		protected override void LastFilter(ref dynamic value)
		{
			if(value < 0d)
				value = 0;
			if(value > 1000_0000_0000)
				value = 1000_0000_0000;
		}
	}
}
