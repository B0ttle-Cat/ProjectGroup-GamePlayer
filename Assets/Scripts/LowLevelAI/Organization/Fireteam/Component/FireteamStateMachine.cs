using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamStateMachine : OdccFiniteStateMachine
	{
		private FireteamStateData stateData;
		sealed public override OdccStateData ThisStateData => stateData;

		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FireteamStateData>(out stateData))
			{
				stateData = ThisContainer.AddData<FireteamStateData>();
			}
		}
		protected override void FSMAwake()
		{
			if(!ThisContainer.TryGetData<FireteamStateData>(out stateData))
			{
				stateData = ThisContainer.AddData<FireteamStateData>();
			}
		}

		protected override void FSMDestroy()
		{

		}

		public void OnSetMoveTarget(IGetLowLevelAIManager manager, int anchorIndex)
		{
			if(manager == null || manager.LowLevelAI == null) return;

			if(!manager.LowLevelAI.TryGetComponent<MapPathPointComputer>(out var computer)) return;

			stateData.MoveTargetPoint = computer.SelectAnchorIndex(anchorIndex);
		}
	}
}
