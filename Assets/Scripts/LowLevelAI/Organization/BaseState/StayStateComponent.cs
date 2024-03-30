using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IStayStateData : IStateData
	{
		public bool IsStay { get; set; }
		public bool HasMoveTarget { get; }
	}
	public class StayStateComponent : OdccStateComponent
	{
		private IStayStateData iStateData;

		protected override void StateEnable()
		{
			iStateData = ThisStateData as IStayStateData;
			iStateData.IsStay = true;
		}

		protected override void StateDisable()
		{
			iStateData.IsStay = false;
		}
		protected override void StateChangeBeforeUpdate()
		{
			if(iStateData.HasMoveTarget)
			{
				OnTransitionState<MovementStateComponent>();
				return;
			}
		}
		protected override void StateUpdate()
		{

		}

	}
}
