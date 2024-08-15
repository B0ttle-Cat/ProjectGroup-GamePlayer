using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitSpawnActor : FireunitCommandActor<SpawnData>
	{
		private IAgentMoveTarget agentMove;
		[SerializeField]
		private bool isEndedSpawn;
		protected override void Disposing()
		{
			base.Disposing();
			CommandData = null;
			agentMove = null;
		}
		public override void BaseActorEnable()
		{
			ThisContainer.TryGetComponent<IAgentMoveTarget>(out agentMove);

			isEndedSpawn = false;
		}
		public override void BaseActorDisable()
		{
			agentMove = null;
		}

		public override void BaseActorUpdate()
		{
			if(isEndedSpawn) return;
			if(CommandData.targetAnchor == null) return;
			if(agentMove == null && !ThisContainer.TryGetComponent<IAgentMoveTarget>(out agentMove)) return;
			UpdateSpawn();
		}


		private void UpdateSpawn()
		{
			Vector3 randomOffset = Random.insideUnitSphere * CommandData.targetRandomRadius;
			Vector3 anchorPosition = CommandData.targetAnchor.ThisPosition();
			Vector3 spawnAroundPoint = randomOffset + GetAroundPosition.GetAroundTeleportationPosition(CommandData.unitIndex, CommandData.totalUnitCount, CommandData.targetRadius, CommandData.targetRandomRadius);

			agentMove.InputMoveTarget(anchorPosition, true);
			agentMove.InputMoveTarget(anchorPosition + spawnAroundPoint);

			isEndedSpawn = true;

			Destroy(this);
		}
	}
}
