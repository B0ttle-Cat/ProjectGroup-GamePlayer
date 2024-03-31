using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IMovementStateData : IStateData
	{
		public bool IsMovement { get; set; }
		public MapPathNode MovePathNode { get; set; }
		public bool HasMoveTarget { get; }
	}

	public class MovementStateComponent : OdccStateComponent
	{
		private IMovementStateData iStateData;
		public MapPathNode movePathNode;

		private FireteamMembers fireteamMembers;
		protected override void StateEnable()
		{
			iStateData = ThisStateData as IMovementStateData;
			iStateData.IsMovement = true;

			fireteamMembers = ThisContainer.GetComponent<FireteamMembers>();
			UpdateMove(iStateData.MovePathNode);
		}

		protected override void StateDisable()
		{
			iStateData.IsMovement = false;

			UpdateMove(null);
			fireteamMembers = null;
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
			var moveTarget = iStateData.MovePathNode;
			if(movePathNode == moveTarget)
			{
				bool isAllStop = true;
				if(fireteamMembers != null)
				{
					fireteamMembers.Foreach(item =>
					{
						if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent) && agent.IsMove)
						{
							isAllStop = false;
						}
					});
				}
				if(isAllStop)
				{
					iStateData.MovePathNode = null;
				}
			}
			else
			{
				UpdateMove(moveTarget);
			}
		}
		private void UpdateMove(MapPathNode moveTarget)
		{
			if(fireteamMembers == null) return;

			movePathNode = moveTarget;

			if(movePathNode == null || movePathNode.ThisPoint == null)
			{
				fireteamMembers.Foreach(item =>
				{
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
					{
						agent.InputMoveStop();
					}
				});
			}
			else
			{

				//Vector3[] aroundPosition = inputNodeTarget.NextNode.ThisPoint.GetRandomAroundPosition(fireteamMembers.Count);
				//int index = 0;
				fireteamMembers.Foreach(item =>
				{
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
					{
						agent.InputMoveTarget(movePathNode);
					}
				});
			}
		}
	}
}
