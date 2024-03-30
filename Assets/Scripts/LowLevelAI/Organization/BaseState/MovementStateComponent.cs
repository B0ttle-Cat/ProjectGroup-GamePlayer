using BC.ODCC;

using UnityEngine;

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
		public MapPathPoint moveTargetPoint;

		private FireteamMembers fireteamMembers;
		protected override void StateEnable()
		{
			iStateData = ThisStateData as IMovementStateData;
			iStateData.IsMovement = true;

			fireteamMembers = ThisContainer.GetComponent<FireteamMembers>();
			UpdateMove(iStateData.MoveTargetPoint);
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
			var moveTarget = iStateData.MoveTargetPoint;
			if(moveTargetPoint == moveTarget)
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
					iStateData.MoveTargetPoint = null;
				}
			}
			else
			{
				UpdateMove(moveTarget);
			}
		}
		private void UpdateMove(MapPathPoint moveTarget)
		{
			if(fireteamMembers == null) return;

			moveTargetPoint = moveTarget;

			if(moveTargetPoint == null)
			{
				fireteamMembers.Foreach(item =>
				{
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
					{
						agent.InputMoveStop();
					}
				});
			}
			else
			{
				Vector3[] aroundPosition = moveTargetPoint.GetRandomAroundPosition(fireteamMembers.Count);
				int index = 0;
				fireteamMembers.Foreach(item =>
				{
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
					{
						agent.InputMoveTarget(aroundPosition[index++]);
					}
				});
			}
		}
	}
}
