using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;
using BC.GamePlayerInterface;
using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;

namespace BC.GamePlayerManager
{
	public abstract class GamePlayer : ComponentBehaviour, IGamePlayer
	{
		protected GamePlayerData playerData;
		protected GamePlayingData playingData;
		protected IGetLowLevelAIManager lowLevelAIManager;
		protected IGetHighLevelAIManager highLevelAIManager;
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

		private OdccQueryCollector fireteamCollector;

		private FactionData factionData;
		private List<FireteamObject> fireteamObjectList;

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

			factionData = ThisContainer.GetData<FactionData>();
			fireteamObjectList = new List<FireteamObject>();
			fireteamCollector = OdccQueryCollector.CreateQueryCollector(
				QuerySystemBuilder.CreateQuery()
					.WithAll<FireteamObject, FireteamData, FireteamStateMachine>()
					.Build())
				.CreateChangeListEvent(InitTeamList, UpdateTeamList);

			ThisContainer.TryGetParentObject<IGetLowLevelAIManager>(out lowLevelAIManager);
			ThisContainer.TryGetParentObject<IGetHighLevelAIManager>(out highLevelAIManager);
			//manager = GetComponet
		}
		public override void BaseDestroy()
		{
			if(fireteamCollector != null)
			{
				fireteamCollector.DeleteChangeListEvent(UpdateTeamList);
				fireteamCollector = null;
			}
			if(fireteamObjectList != null)
			{
				fireteamObjectList.Clear();
				fireteamObjectList = null;
			}
		}

		private void InitTeamList(IEnumerable<ObjectBehaviour> enumerable)
		{
			int ThisFactionIndex = factionData.FactionIndex;
			fireteamObjectList = enumerable.Select(item => item as FireteamObject)
				.Where(item => item != null && item.ThisContainer.GetData<FireteamData>().FactionIndex == ThisFactionIndex)
				.ToList();
		}
		private void UpdateTeamList(ObjectBehaviour behaviour, bool isAdded)
		{
			if(isAdded)
			{
				if(behaviour is FireteamObject fireteamObject)
				{
					FireteamData fireteamData = fireteamObject.ThisContainer.GetData<FireteamData>();
					if(fireteamData.FactionIndex == factionData.FactionIndex && !fireteamObjectList.Contains(fireteamObject))
					{
						fireteamObjectList.Add(fireteamObject);
					}
				}
			}
			else
			{
				if(behaviour is FireteamObject fireteamObject)
				{
					fireteamObjectList.Remove(fireteamObject);
				}
			}
		}


		public virtual void OnSelectFireteam(int selectTeamIndex)
		{
			if(playingData == null && ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				playingData = data;
			}
			if(playingData == null) return;

			playingData.CurrentSelectTeamIndex = selectTeamIndex;
		}
		public virtual void OnSetMoveTarget(int anchorIndex)
		{
			if(playingData == null && ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				playingData = data;
			}
			if(playingData == null) return;

			int currentSelectTeam = playingData.CurrentSelectTeamIndex;
			if(currentSelectTeam < 0) return;
			int findSelectTeamIndex = fireteamObjectList.FindIndex(item=>item.ThisContainer.GetData<FireteamData>().TeamIndex == currentSelectTeam);
			if(findSelectTeamIndex < 0) return;

			FireteamObject selectTeamObject = fireteamObjectList[findSelectTeamIndex];

			if(selectTeamObject.ThisContainer.TryGetComponent<FireteamStateMachine>(out var movement))
			{
				movement.OnSetMoveTarget(lowLevelAIManager, anchorIndex);
			}

		}
	}
}
