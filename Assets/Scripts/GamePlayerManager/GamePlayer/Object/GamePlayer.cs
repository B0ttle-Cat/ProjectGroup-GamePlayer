using System.Collections.Generic;
using System.Linq;

using BC.GamePlayerInterface;
using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;

namespace BC.GamePlayerManager
{
	public abstract class GamePlayer : ComponentBehaviour, IGamePlayer
	{
		protected IGetLowLevelAIManager lowLevelAIManager;
		protected IGetHighLevelAIManager highLevelAIManager;
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<GamePlayerData>(out _))
			{
				ThisContainer.AddData<GamePlayerData>();
			}
			if(!ThisContainer.TryGetData<GamePlayingData>(out _))
			{
				ThisContainer.AddData<GamePlayingData>();
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
			if(ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				data.CurrentSelectTeamIndex = selectTeamIndex;
			}
		}
		public virtual void OnSetMoveTarget(int anchorIndex)
		{
			if(ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				int currentSelectTeam = data.CurrentSelectTeamIndex;
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
}
