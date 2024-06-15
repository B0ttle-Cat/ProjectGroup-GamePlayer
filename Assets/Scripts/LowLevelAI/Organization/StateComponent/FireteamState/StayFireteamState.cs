using BC.ODCC;


using UnityEngine;

namespace BC.LowLevelAI
{
	public interface IStayStateData : IStateData
	{
		public bool IsStay { get; set; }
		public bool HasMoveTarget { get; }
	}
	[RequireComponent(typeof(FireteamStateMachine))]
	public class StayFireteamState : OdccStateComponent
	{
		private IStayStateData iStateData;

		protected override void Disposing()
		{
			base.Disposing();
			iStateData = null;
		}

		protected override void StateEnable()
		{
			iStateData = ThisStateData as IStayStateData;
			iStateData.IsStay = true;

		}

		protected override void StateDisable()
		{
			iStateData.IsStay = false;
		}
		protected override void StateChangeInHere()
		{
			if(iStateData.HasMoveTarget)
			{
				//OnTransitionState<MovementFireteamState>();
				return;
			}
		}
		protected override void StateUpdate()
		{

		}

	}
}
