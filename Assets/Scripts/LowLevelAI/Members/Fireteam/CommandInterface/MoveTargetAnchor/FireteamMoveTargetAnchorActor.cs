using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMoveTargetAnchorActor : FireteamCommandActor<FireteamMoveTargetAnchorData>
	{
		public override void BaseActorEnable()
		{
			StartMove();
		}
		public override void BaseActorDisable()
		{
			StopMove();
		}
		public override void BaseActorLateUpdate()
		{
			if(CheckIsMoveEnd())
			{
				Destroy(this);
			}
		}



		private void StartMove()
		{
			MapPathNode movePathNode = CommandData.MovePathNode;

			if(movePathNode != null && movePathNode.ThisPoint != null)
			{
				var lastNode = movePathNode.EndedNode;
				var lastPrevNode = movePathNode.EndedNode.PrevNode;

				float formationRadius = 2f;
				float formationRandomRadius = 1f;

				Vector3 randomOffset = Random.insideUnitSphere * 2f;
				Vector3 lastPosition = lastNode.ThisPoint.ThisPosition() + randomOffset;
				Vector3 lastPrevPosition = lastPrevNode == null ? FireteamMembers.CenterPosition : lastPrevNode.ThisPoint.ThisPosition();
				Vector3 angleNormal = (lastPosition - lastPrevPosition).normalized;

				Vector3 anchorPosition = lastNode.ThisPoint.ThisAnchor.ThisPosition();
				Vector3[] formationPosition = GetAroundPosition.GetAroundMovePosition(FireteamMembers.Count, formationRadius, formationRandomRadius, angleNormal);

				FireteamMembers.Foreach((item, index) => {
					if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
					{
						agent.InputMoveTarget(movePathNode, randomOffset + formationPosition[index]);
					}
				});
			}
		}
		private void StopMove()
		{
			FireteamMembers.Foreach(item => {
				if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
				{
					agent.InputMoveStop();
				}
			});
		}
		private bool CheckIsMoveEnd()
		{
			if(FireteamMembers.HasCondition(member => member.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove)))
			{
				return false;
			}
			return true;
		}
	}
}
