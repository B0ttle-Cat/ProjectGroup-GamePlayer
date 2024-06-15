using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public interface ILowLevelPlayerInterface : IOdccComponent
	{
		int OnLowPlayingInterface_GetSelectedFireteam();
		void OnLowPlayingInterface_SelectFireteam(int selectTeamIndex);
		void OnLowPlayingInterface_SetMoveTarget(int anchorIndex, int selectTeamIndex = -1);
		void OnLowPlayingInterface_SpawnTeamToAnchor(int anchorIndex, int selectTeamIndex = -1);
	}

	public interface IHighLevelPlayerInterface : IOdccComponent
	{
		int OnHighPlayingInterface_GetSelectedFireteam();
	}

	public abstract class GamePlayerInterface : ComponentBehaviour, ILowLevelPlayerInterface
	{
		#region Base
		protected GamePlayerData playerData;
		protected GamePlayingData playingData;
		protected IGetLowLevelAIManager lowLevelAIManager;
		protected IGetHighLevelAIManager highLevelAIManager;
		protected FactionMemberCollector memberCollector;
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<GamePlayerData>(out playerData))
			{
				playerData = ThisContainer.AddData<GamePlayerData>();
			}
			if(!ThisContainer.TryGetData<GamePlayingData>(out playingData))
			{
				playingData = ThisContainer.AddData<GamePlayingData>();
			}
			ThisContainer.TryGetParentObject<IGetLowLevelAIManager>(out lowLevelAIManager);
			ThisContainer.TryGetParentObject<IGetHighLevelAIManager>(out highLevelAIManager);
		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<GamePlayerData>(out playerData))
			{
				playerData = ThisContainer.AddData<GamePlayerData>();
			}
			if(!ThisContainer.TryGetData<GamePlayingData>(out playingData))
			{
				playingData = ThisContainer.AddData<GamePlayingData>();
			}

			ThisContainer.TryGetParentObject<IGetLowLevelAIManager>(out lowLevelAIManager);
			ThisContainer.TryGetParentObject<IGetHighLevelAIManager>(out highLevelAIManager);

			ThisContainer.TryGetComponent<FactionMemberCollector>(out memberCollector);
		}
		public override void BaseDestroy()
		{

		}
		protected override void Disposing()
		{
			base.Disposing();
			playerData = null;
			playingData = null;
			lowLevelAIManager = null;
			highLevelAIManager = null;
			memberCollector = null;
		}
		#endregion

		#region ILowLevelPlayingInterface
		int ILowLevelPlayerInterface.OnLowPlayingInterface_GetSelectedFireteam()
		{
			return GetSelectedFireteam();
		}
		[Button("OnLowPlayingInterface_SelectFireteam")]
		void ILowLevelPlayerInterface.OnLowPlayingInterface_SelectFireteam(int selectTeamIndex)
		{
			SelectFireteam(selectTeamIndex);
		}
		[Button("OnLowPlayingInterface_SetMoveTarget")]
		void ILowLevelPlayerInterface.OnLowPlayingInterface_SetMoveTarget(int movementToAnchor, int tempSelectTeamIndex = -1)
		{
			SetMoveTarget(movementToAnchor, tempSelectTeamIndex);
		}
		[Button("OnLowPlayingInterface_SpawnTeamToAnchor")]
		void ILowLevelPlayerInterface.OnLowPlayingInterface_SpawnTeamToAnchor(int teamSpawnToAnchor, int tempSelectTeamIndex = -1)
		{
			SpawnOnAnchor(teamSpawnToAnchor, tempSelectTeamIndex);
		}
		#endregion

		#region LowLevelPlaying Function
		public virtual void SelectFireteam(int selectTeamIndex)
		{
			if(BreakPlayingData()) return;

			playingData.CurrentSelectTeamIndex = selectTeamIndex;
		}
		public virtual int GetSelectedFireteam()
		{
			if(BreakPlayingData()) return -1;

			return playingData.CurrentSelectTeamIndex;
		}
		public virtual void SetMoveTarget(int anchorIndex, int selectTeamIndex)
		{
			if(BreakLowLevelAIManager() || BreakPlayingData()) return;
			int currentSelectTeam = selectTeamIndex >= 0 ? selectTeamIndex : GetSelectedFireteam();

			if(!TrySelectFireteam(currentSelectTeam, out var selectTeamObject)) return;
			if(!TryGetTeamControllerAndCollector(selectTeamObject, out var teamCommand, out var teamMembers)) return;

			if(!TryGetMapPathPointComputer(out var computer)) return;
			if(!TryGetPathPointToNode(computer, anchorIndex, teamMembers, out var pathNode)) return;

			teamCommand.OnTeamCommand_SetMoveTarget(pathNode);
		}
		public void SpawnOnAnchor(int anchorIndex, int selectTeamIndex)
		{
			if(BreakLowLevelAIManager() || BreakPlayingData()) return;
			int currentSelectTeam = selectTeamIndex >= 0 ? selectTeamIndex : GetSelectedFireteam();

			if(!TrySelectFireteam(currentSelectTeam, out FireteamObject selectTeamObject)) return;
			if(!TryGetTeamControllerAndCollector(selectTeamObject, out var teamCommand, out var teamMembers)) return;

			if(!TryGetMapPathPointComputer(out var computer)) return;
			if(TrySelectAnchorIndex(computer, anchorIndex, out var spawnAnchor)) return;

			teamCommand.OnTeamCommand_SpawnOnAnchor(spawnAnchor);
		}
		#endregion
		#region LowLevelPlaying Function-Utils
		private bool BreakLowLevelAIManager() => (lowLevelAIManager == null || lowLevelAIManager.LowLevelAI == null);
		private bool BreakPlayingData() => (playingData == null && !ThisContainer.TryGetData<GamePlayingData>(out playingData));
		private bool TrySelectFireteam(int currentSelectTeam, out FireteamObject select)
		{
			if(currentSelectTeam >= 0 && memberCollector.TryFindFireteam(currentSelectTeam, out select))
			{
				return true;
			}
			select = null;
			return false;
		}
		private bool TryGetTeamControllerAndCollector(FireteamObject teamObject, out IFireteamCommandInterface command, out FireteamMemberCollector members)
		{
			command = null;
			members = null;
			if(!teamObject.ThisContainer.TryGetComponent<IFireteamCommandInterface>(out command)) return false;
			if(!teamObject.ThisContainer.TryGetComponent<FireteamMemberCollector>(out members)) return false;
			return command != null && members != null;
		}
		private bool TryGetMapPathPointComputer(out MapPathPointComputer computer)
		{
			computer = null;
			if(!lowLevelAIManager.MapStage.ThisContainer.TryGetComponent<MapPathPointComputer>(out computer)) return false;
			return computer != null;
		}
		private bool TryGetPathPointToNode(MapPathPointComputer computer, int anchorIndex, FireteamMemberCollector members, out MapPathNode pathNode)
		{
			pathNode = null;
			if(!computer.TryGetClosedPathPoint(members.CenterPosition, out var closedPathPoint)) return false;
			if(!computer.TrySelectPathPointIndex(anchorIndex, out var moveTargetPoint)) return false;
			if(moveTargetPoint == null) return false;
			if(!closedPathPoint.CalculatePath(moveTargetPoint, out pathNode)) return false;
			return pathNode != null;
		}
		private bool TrySelectAnchorIndex(MapPathPointComputer computer, int selectIndex, out MapAnchor mapAnchor)
		{
			mapAnchor = null;
			if(computer.TrySelectAnchorIndex(selectIndex, out mapAnchor)) return false;
			return mapAnchor != null;
		}
		#endregion
	}
}
