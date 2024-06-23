using BC.ODCC;

namespace BC.OdccBase
{
	public abstract class AbilityObject<TNumber, TType> : ObjectBehaviour
		where TNumber : NumberValue<TType>, new()
		where TType : unmanaged
	{
		protected TNumber ThisValue;
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
			if(!ThisContainer.TryGetData<TNumber>(out _))
			{
				ThisContainer.AddData<TNumber>();
			}
		}
		public sealed override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<TNumber>(out ThisValue))
			{
				ThisValue = ThisContainer.AddData<TNumber>();
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
		public virtual TType OnCalculation()
		{
			return Calculation(ThisValue);
		}

		protected dynamic Calculation(TNumber number)
		{
			dynamic value = number.Value;
			FirstFilter(ref value);
			ThisContainer.CallActionAllComponent<AbilityCalculationModule>(module => module.Calculation(ref value));
			LastFilter(ref value);
			return value;
		}
		protected virtual void FirstFilter(ref dynamic value) { }
		protected virtual void LastFilter(ref dynamic value) { }
	}
}
