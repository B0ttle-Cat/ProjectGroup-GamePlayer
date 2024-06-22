using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IFireteamCommandInterface : IOdccComponent
	{
		public void OnTeamCommand_MoveTargetAnchor(FireteamMemberCollector memberCollector, MapPathNode target);
		public void OnTeamCommand_SpawnOnAnchor(FireteamMemberCollector memberCollector, MapAnchor target);
		public void OnTeamCommand_TeleportationOnAnchor(FireteamMemberCollector memberCollector, MapAnchor target);
	}

	public class FireteamCommandController : ComponentBehaviour, IFireteamCommandInterface
	{
		#region IFireteamCommandInterface
		void IFireteamCommandInterface.OnTeamCommand_MoveTargetAnchor(FireteamMemberCollector _memberCollector, MapPathNode target)
		{
			FireteamMemberCollector memberCollector = _memberCollector;
			if(memberCollector == null && !ThisContainer.TryGetComponent<FireteamMemberCollector>(out memberCollector)) return;
			MoveTargetAnchor(memberCollector, target);
		}
		void IFireteamCommandInterface.OnTeamCommand_SpawnOnAnchor(FireteamMemberCollector _memberCollector, MapAnchor target)
		{
			FireteamMemberCollector memberCollector = _memberCollector;
			if(memberCollector == null && !ThisContainer.TryGetComponent<FireteamMemberCollector>(out memberCollector)) return;
			SpawnOnAnchor(memberCollector, target);
		}
		void IFireteamCommandInterface.OnTeamCommand_TeleportationOnAnchor(FireteamMemberCollector _memberCollector, MapAnchor target)
		{
			FireteamMemberCollector memberCollector = _memberCollector;
			if(memberCollector == null && !ThisContainer.TryGetComponent<FireteamMemberCollector>(out memberCollector)) return;
			TeleportationOnAnchor(memberCollector, target);
		}
		#endregion
		#region FireteamCommand Function
		protected void MoveTargetAnchor(FireteamMemberCollector memberCollector, MapPathNode target)
		{
			if(ThisContainer.TryGetChildObject<FireteamMoveTargetAnchorCommander>(out var oldCommander))
			{
				Destroy(oldCommander.gameObject);
			}

			var commander = ThisContainer.AddChildObject<FireteamMoveTargetAnchorCommander>(false);
			var actor = commander.ThisContainer.AddComponent<FireteamMoveTargetAnchorActor>();
			var data = commander.ThisContainer.AddData<FireteamMoveTargetAnchorData>();

			data.Members = memberCollector;
			data.MovePathNode = target;

			commander.gameObject.SetActive(true);
		}
		protected void SpawnOnAnchor(FireteamMemberCollector memberCollector, MapAnchor target)
		{
			if(ThisContainer.TryGetChildObject<FireteamSpawnOnAnchorCommander>(out var oldCommander))
			{
				Destroy(oldCommander.gameObject);
			}

			var commander = ThisContainer.AddChildObject<FireteamSpawnOnAnchorCommander>(false);
			var actor = commander.ThisContainer.AddComponent<FireteamSpawnOnAnchorActor>();
			var data = commander.ThisContainer.AddData<FireteamSpawnOnAnchorData>();

			data.Members = memberCollector;
			data.SpawnAnchor = target;

			commander.gameObject.SetActive(true);
		}
		protected void TeleportationOnAnchor(FireteamMemberCollector memberCollector, MapAnchor target)
		{
			if(ThisContainer.TryGetChildObject<FireteamTeleportationOnAnchorCommander>(out var oldCommander))
			{
				Destroy(oldCommander.gameObject);
			}

			var commander = ThisContainer.AddChildObject<FireteamTeleportationOnAnchorCommander>(false);
			var actor = commander.ThisContainer.AddComponent<FireteamTeleportationOnAnchorActor>();
			var data = commander.ThisContainer.AddData<FireteamTeleportationOnAnchorData>();

			data.Members = memberCollector;
			data.TeleportationAnchor = target;

			commander.gameObject.SetActive(true);
		}
		#endregion
	}
}
