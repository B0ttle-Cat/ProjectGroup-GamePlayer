using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IMovementStateData : IStateData
	{
		public bool IsMovement { get; set; }
		public MapPathPoint MoveTargetPoint { get; set; }
		public bool HasMoveTarget { get; }
	}

	public class MovementStateComponent : OdccStateComponent
	{
		private IMovementStateData iStateData;

		protected override void StateEnable()
		{
			iStateData = ThisStateData as IMovementStateData;
			iStateData.IsMovement = true;
		}

		protected override void StateDisable()
		{
			iStateData.IsMovement = false;
		}
		protected override void StateChangeBeforeUpdate()
		{
			if(!iStateData.HasMoveTarget)
			{
				OnTransitionState<StayStateComponent>();
				return;
			}
		}
		protected override void StateUpdate()
		{

		}

	}
}
