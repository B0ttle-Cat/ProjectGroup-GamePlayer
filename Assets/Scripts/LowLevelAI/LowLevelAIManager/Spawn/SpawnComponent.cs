using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class SpawnComponent : ComponentBehaviour
	{
		private SpawnData spawnData;
		private IAgentMoveTarget agentMove;
		[SerializeField]
		private bool isEndedSpawn;
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			spawnData = null;
			agentMove = null;
		}
		public override void BaseAwake()
		{
			base.BaseAwake();

			spawnData = null;
			agentMove = null;
			ThisContainer.TryGetData<SpawnData>(out spawnData);
			ThisContainer.TryGetComponent<IAgentMoveTarget>(out agentMove);

			isEndedSpawn = false;
		}

		public override void BaseUpdate()
		{
			base.BaseUpdate();

			UpdateSpawn();
		}


		private void UpdateSpawn()
		{
			if(isEndedSpawn) return;

			if(spawnData is null && !ThisContainer.TryGetData<SpawnData>(out spawnData)) return;
			if(agentMove is null && !ThisContainer.TryGetComponent<IAgentMoveTarget>(out agentMove)) return;

			if(spawnData.spawnAnchorTarget == null) return;

			Vector3 anchorPosition = spawnData.spawnAnchorTarget.ThisPosition();
			Vector3 spawnAroundPoint = GetAroundPosition.GetAroundSpwanPosition(spawnData.spawnUnitIndex, spawnData.spawnUnitCount, spawnData.spaenRadius);

			agentMove.InputMoveTarget(anchorPosition, true);
			agentMove.InputMoveTarget(anchorPosition + spawnAroundPoint);

			isEndedSpawn = true;

			Destroy(this);
		}
	}
}
