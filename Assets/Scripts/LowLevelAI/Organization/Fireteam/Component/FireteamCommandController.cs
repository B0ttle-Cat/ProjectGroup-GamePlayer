using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public interface IFireteamCommandInterface : IOdccComponent
	{
		public void OnTeamCommand_SetMoveTarget(MapPathNode pathNode);
		public void OnTeamCommand_SpawnOnAnchor(MapAnchor spawnAnchor);
		public void TeamCommand_TeleportationOnAnchor(MapAnchor spawnAnchor);
	}

	public class FireteamCommandController : ComponentBehaviour, IFireteamCommandInterface
	{
		FireteamMemberCollector memberCollector;
		public override async void BaseAwake()
		{
			await ThisContainer.AwaitGetComponent<FireteamMemberCollector>((comp) => memberCollector = comp);
		}
		public override void BaseDestroy()
		{
			memberCollector = null;
		}

		#region IFireteamCommandInterface
		void IFireteamCommandInterface.OnTeamCommand_SetMoveTarget(MapPathNode pathNode)
		{
			SetMoveTarget(pathNode);
		}
		void IFireteamCommandInterface.OnTeamCommand_SpawnOnAnchor(MapAnchor spawnAnchor)
		{
			SpawnOnAnchor(spawnAnchor);
		}
		void IFireteamCommandInterface.TeamCommand_TeleportationOnAnchor(MapAnchor spawnAnchor)
		{
			TeleportationOnAnchor(spawnAnchor);
		}
		#endregion
		#region FireteamCommand Function
		protected void SetMoveTarget(MapPathNode pathNode)
		{
			if(memberCollector == null) return;

			if(ThisContainer.TryGetChildObject<FireteamMovementCommander>(out var oldCommander))
			{
				Destroy(oldCommander.gameObject);
			}

			GameObject newObject = new GameObject(nameof(FireteamMovementCommander));
			newObject.SetActive(false);
			newObject.transform.parent = ThisTransform;

			FireteamMovementCommander commander = newObject.AddComponent<FireteamMovementCommander>();
			FireteamMovementActor actor = commander.ThisContainer.AddComponent<FireteamMovementActor>();
			FireteamMovementData data = commander.ThisContainer.AddData<FireteamMovementData>();

			data.Members = memberCollector;
			data.MovePathNode = pathNode;

			newObject.SetActive(true);
		}
		protected void SpawnOnAnchor(MapAnchor spawnAnchor)
		{
			if(memberCollector == null) return;

			int index = 0;
			int total = memberCollector.Count;
			memberCollector.Foreach(unitObject => {
				var spawn = new SpawnData() {
					spawnAnchorTarget = spawnAnchor,
					spawnUnitCount = total,
					spawnUnitIndex = index++,
					spawnRadius = 2f,
				};
				unitObject.ThisContainer.RemoveData<SpawnData>();
				unitObject.ThisContainer.RemoveComponent<SpawnComponent>();

				unitObject.ThisContainer.AddData<SpawnData>(spawn);
				unitObject.ThisContainer.AddComponent<SpawnComponent>();
			});
		}
		protected void TeleportationOnAnchor(MapAnchor spawnAnchor)
		{
			if(memberCollector == null) return;

			int index = 0;
			int total = memberCollector.Count;
			memberCollector.Foreach(unitObject => {
				var spawn = new SpawnData() {
					spawnAnchorTarget = spawnAnchor,
					spawnUnitCount = total,
					spawnUnitIndex = index++,
					spawnRadius = 2f,
				};
				unitObject.ThisContainer.RemoveData<SpawnData>();
				unitObject.ThisContainer.RemoveComponent<SpawnComponent>();

				unitObject.ThisContainer.AddData<SpawnData>(spawn);
				unitObject.ThisContainer.AddComponent<SpawnComponent>();
			});
		}
		#endregion
	}
}
