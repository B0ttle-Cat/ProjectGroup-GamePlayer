using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamCommandController : ComponentBehaviour
	{
		public void OnSetMoveTarget(FireteamMembers members, MapPathNode pathNode)
		{
			if(ThisContainer.TryGetChildObject<FireteamMovementCommander>(out var oldCommander))
			{
				Destroy(oldCommander.gameObject);
			}

			GameObject newObject = new GameObject(nameof(FireteamMovementCommander));
			newObject.SetActive(false);

			FireteamMovementCommander commander = newObject.AddComponent<FireteamMovementCommander>();
			FireteamMovementActor actor = commander.ThisContainer.AddComponent<FireteamMovementActor>();
			FireteamMovementData data = commander.ThisContainer.AddData<FireteamMovementData>();

			data.Members = members;
			data.MovePathNode = pathNode;

			newObject.SetActive(true);
		}

		public void OnTeamSpawnTarget(FireteamMembers members, MapAnchor spawnAnchor)
		{
			int index = 0;
			members.Foreach(unitObject => {
				var spawn = new SpawnData() {
					spawnAnchorTarget = spawnAnchor,
					spawnUnitCount = members.Count,
					spawnUnitIndex = index++,
					spaenRadius = 2f,
				};
				unitObject.ThisContainer.RemoveData<SpawnData>();
				unitObject.ThisContainer.RemoveComponent<SpawnComponent>();

				unitObject.ThisContainer.AddData<SpawnData>(spawn);
				unitObject.ThisContainer.AddComponent<SpawnComponent>();
			});
		}
	}
}
