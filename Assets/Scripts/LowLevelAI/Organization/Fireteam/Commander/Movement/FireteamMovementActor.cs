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

		FireteamMemberCollector fireteamMembers;
		FireteamMovementData commanderData;

		public override async void BaseEnable()
		{
			base.BaseEnable();

			fireteamMembers = null;
			commanderData = null;

			commanderData = await ThisContainer.AwaitGetData<FireteamMovementData>(null, DisableCancelToken);
			if(ThisContainer.ParentContainer != null)
				fireteamMembers = await ThisContainer.ParentContainer?.AwaitGetComponent<FireteamMemberCollector>(null, DisableCancelToken);

			StartMove();
		}
		public override void BaseDisable()
		{
			base.BaseDisable();

			StopMove();

			fireteamMembers = null;
			commanderData = null;
		}

		public override void BaseUpdate()
		{
			if(fireteamMembers == null) return;
			if(commanderData == null) return;

			if(!CheckIsMove())
			{
				Destroy(this);
			}
		}

		private void StartMove()
		{
			if(fireteamMembers == null) return;
			if(commanderData == null) return;
			MapPathNode movePathNode = commanderData.MovePathNode;

			if(movePathNode != null && movePathNode.ThisPoint != null)
			{
				var lastNode = movePathNode.EndedNode;
				var lastPrevNode = movePathNode.EndedNode.PrevNode;

				float formationRadius = 2f;
				float formationRandomRadius = 1f;

				Vector3 randomOffset = Random.insideUnitSphere * 2f;
				Vector3 lastPosition = lastNode.ThisPoint.ThisPosition() + Random.insideUnitSphere * formationRandomRadius;
				Vector3 lastPrevPosition = lastPrevNode == null ? fireteamMembers.CenterPosition : lastPrevNode.ThisPoint.ThisPosition();
				Vector3 angleNormal = (lastPosition - lastPrevPosition).normalized;

				Vector3 anchorPosition = lastNode.ThisPoint.ThisAnchor.ThisPosition();
				Vector3[] formationPosition = GetAroundPosition.GetAroundMovePosition(fireteamMembers.Count, formationRadius, formationRandomRadius, angleNormal);

				fireteamMembers.Foreach((item, index) => {
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
					{
						agent.InputMoveTarget(movePathNode, randomOffset + formationPosition[index]);
					}
				});
			}
		}
		private void StopMove()
		{
			if(fireteamMembers == null) return;

			fireteamMembers.Foreach(item => {
				if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
				{
					agent.InputMoveStop();
				}
			});
		}
		private bool CheckIsMove()
		{
			if(fireteamMembers == null) return false;

			bool isMove = false;
			int count = fireteamMembers.Count;
			for(int i = 0 ; i < count ; i++)
			{
				if(fireteamMembers[i].ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
				{
					isMove = true;
					break;
				}
			}
			return isMove;
		}
	}
}
