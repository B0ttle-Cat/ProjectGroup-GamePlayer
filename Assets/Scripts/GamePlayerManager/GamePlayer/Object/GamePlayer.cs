using System.Collections.Generic;
using System.Data;
using System.Linq;

using BC.GamePlayerInterface;
using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

namespace BC.GamePlayerManager
{
	public abstract class GamePlayer : ComponentBehaviour, IGamePlayingInterface
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
		public bool TrySelectFireteam(int currentSelectTeam, out FireteamObject select)
		{
			select = null;
			if(currentSelectTeam < 0) return false;
			if(fireteamObjectList == null) return false;
			currentSelectTeam = fireteamObjectList.FindIndex(item => item.ThisContainer.GetData<FireteamData>().TeamIndex == currentSelectTeam);
			if(currentSelectTeam < 0) return false;

			select = fireteamObjectList[currentSelectTeam];
			return select is not null;
		}
		public virtual void OnSetMoveTarget(int anchorIndex, int? selectTeamIndex = null)
		{
			if(playingData == null && ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				playingData = data;
			}
			if(playingData == null) return;

			int currentSelectTeam = selectTeamIndex ?? playingData.CurrentSelectTeamIndex;
			if(TrySelectFireteam(currentSelectTeam, out var selectTeamObject))
			{
				if(selectTeamObject.ThisContainer.TryGetComponent<FireteamStateMachine>(out var statemachine))
				{
					statemachine.OnSetMoveTarget(lowLevelAIManager, anchorIndex);
				}
			}
		}

		public void OnSpawnTeamToAnchor(int anchorIndex, int? selectTeamIndex = null)
		{
			if(playingData == null && ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				playingData = data;
			}
			if(playingData == null) return;


			int currentSelectTeam = selectTeamIndex ?? playingData.CurrentSelectTeamIndex;
			if(TrySelectFireteam(currentSelectTeam, out var selectTeamObject))
			{
				if(selectTeamObject.ThisContainer.TryGetComponent<FireteamStateMachine>(out var statemachine))
				{
					statemachine.OnTeamSpawnTarget(lowLevelAIManager, anchorIndex);
				}
			}
		}
		public void OnSpawnUnitIndex(int unitIndex, int? selectTeamIndex = null)
		{

		}
	}
}
