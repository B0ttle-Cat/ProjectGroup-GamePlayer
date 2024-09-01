using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class TeleportationComponent : FireunitCommandActor<TeleportationData>
	{
		private IUnitIMovementAgent agentMove;
		[SerializeField]
		private bool isEnded;
		protected override void Disposing()
		{
			base.Disposing();
			agentMove = null;
		}
		public override void BaseActorEnable()
		{
			isEnded = false;
		}
		public override void BaseActorDisable()
		{
			agentMove = null;
		}

		public override void BaseActorUpdate()
		{
			if(isEnded) return;
			if(CommandData.targetAnchor == null) return;
			if(agentMove is null && !ThisContainer.TryGetComponent<IUnitIMovementAgent>(out agentMove)) return;
			UpdateTeleportation();
		}


		private void UpdateTeleportation()
		{
			Vector3 randomOffset = Random.insideUnitSphere * CommandData.targetRandomRadius;
			Vector3 anchorPosition = CommandData.targetAnchor.ThisPosition();
			Vector3 teleportationAroundPoint = randomOffset + GetAroundPosition.GetAroundTeleportationPosition(CommandData.unitIndex, CommandData.totalUnitCount, CommandData.targetRadius, CommandData.targetRandomRadius);

			agentMove.InputMoveTarget(anchorPosition + teleportationAroundPoint, true);

			CommandData = null;
			Destroy(this);
		}
	}
}
