using BC.ODCC;

namespace BC.OdccBase
{
	public abstract class AbilityObject : ObjectBehaviour
	{
		protected AbilityData ThisAbility;
		public sealed override void BaseReset()
		{
			base.BaseReset();
		}
		public sealed override void BaseValidate()
		{
			base.BaseValidate();
			BaseAbilityValidate();
		}
		public virtual void BaseAbilityValidate()
		{
			if(!ThisContainer.TryGetData<AbilityData>(out _))
			{
				ThisContainer.AddData<AbilityData>();
			}
		}
		public sealed override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<AbilityData>(out var ThisAbility))
			{
				ThisAbility = ThisContainer.AddData<AbilityData>();
			}
			BaseAbilityAwake();
		}
		public sealed override void BaseDestroy()
		{
			base.BaseDestroy();
			BaseAbilityDestroy();
		}
		public abstract void BaseAbilityAwake();
		public abstract void BaseAbilityDestroy();
		public abstract void OnCalculation();

		protected double Calculation(double value)
		{
			FirstFilter(ref value);
			ThisContainer.CallActionAllComponent<AbilityCalculationModule>(module => module.Calculation(ref value));
			LastFilter(ref value);
			return value;
		}
		protected virtual void FirstFilter(ref double value) { }
		protected virtual void LastFilter(ref double value) { }
	}
}
