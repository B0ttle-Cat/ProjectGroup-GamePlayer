using BC.ODCC;

using UnityEngine;

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

			if(!manager.LowLevelAI.ThisContainer.TryGetComponent<MapPathPointComputer>(out var computer)) return;

			if(!ThisContainer.TryGetComponent<FireteamMembers>(out var members)) return;

			Vector3 center = members.CenterPosition();

			if(!computer.TryGetClosedPathPoint(center, out var closedPathPoint)) return;

			var moveTargetPoint = computer.SelectAnchorIndex(anchorIndex);
			if(closedPathPoint ==null || moveTargetPoint == null) return;


			if(!closedPathPoint.CalculatePath(moveTargetPoint, out var pathNode)) return;
			stateData.MovePathNode = pathNode;
		}
	}
}
