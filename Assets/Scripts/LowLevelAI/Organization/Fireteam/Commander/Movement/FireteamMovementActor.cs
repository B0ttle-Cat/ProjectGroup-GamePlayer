using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMovementActor : FireteamCommandActor
	{
		public override void BaseReset()
		{
			base.BaseReset();
			if(!ThisContainer.TryGetData<FireteamMovementData>(out _))
			{
				ThisContainer.AddData<FireteamMovementData>();
			}
		}
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FireteamMovementData>(out _))
			{
				ThisContainer.AddData<FireteamMovementData>();
			}
		}

		FireteamMembers fireteamMembers;
		FireteamMovementData commanderData;

		public override void BaseEnable()
		{
			base.BaseEnable();

			fireteamMembers = null;
			commanderData = null;

			ThisContainer.AwaitGetData<FireteamMovementData>((data) => {
				commanderData = data;
				fireteamMembers = commanderData.Members;
			}, item => item.HasMoveTarget, DisableCancelToken);
		}
		public override void BaseDisable()
		{
			base.BaseDisable();

			fireteamMembers = null;
			commanderData = null;
		}

		public override void BaseUpdate()
		{
			if(fireteamMembers == null) return;
			if(commanderData == null) return;

			base.BaseUpdate();
		}

		protected void StateUpdate()
		{
			if(fireteamMembers.Count == 0)
			{
				commanderData.MovePathNode = null;
				return;
			}

			if(commanderData.HasMoveTarget)
			{
				UpdateMove(commanderData.MovePathNode);
			}
		}
		private void UpdateMove(MapPathNode movePathNode)
		{
			if(movePathNode == null || movePathNode.ThisPoint == null)
			{
				fireteamMembers.Foreach(item => {
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
					{
						agent.InputMoveStop();
					}
				});
			}
			else
			{
				var lastNode = movePathNode.EndedNode;
				var lastPrevNode = movePathNode.EndedNode.PrevNode;

				Vector3 lastPosition = lastNode.ThisPoint.ThisPosition();
				Vector3 lastPrevPosition = lastPrevNode == null ? fireteamMembers.CenterPosition : lastPrevNode.ThisPoint.ThisPosition();
				Vector3 angleNormal = (lastPosition - lastPrevPosition).normalized;

				Vector3 anchorPosition = lastNode.ThisPoint.ThisAnchor.ThisPosition();
				Vector3[] spawnAroundPoints = GetAroundPosition.GetAroundMovePosition(fireteamMembers.Count, 2f, angleNormal);

				fireteamMembers.Foreach((item, index) => {
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
					{
						agent.InputMoveTarget(movePathNode, spawnAroundPoints[index] + anchorPosition);
					}
				});
			}
		}
	}
}
