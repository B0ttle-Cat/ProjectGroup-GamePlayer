using System.Collections.Generic;
using System.Linq;

using BC.LowLevelAI;
using BC.ODCC;

namespace BC.GamePlayerManager
{
	public abstract class GamePlayer : ComponentBehaviour
	{
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
					.WithAll<FireteamObject, FireteamData, FireteamController>()
					.Build())
				.CreateChangeListEvent(InitTeamList, UpdateTeamList);
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

		public virtual void OnMovementToAnchor(int anchorIndex)
		{
			if(ThisContainer.TryGetData<GamePlayingData>(out var data))
			{
				int currentSelectTeam = data.CurrentSelectTeamIndex;
				if(currentSelectTeam < 0) return;
				int findSelectTeamIndex = fireteamObjectList.FindIndex(item=>item.ThisContainer.GetData<FireteamData>().TeamIndex == currentSelectTeam);
				if(findSelectTeamIndex < 0) return;

				FireteamObject selectTeamObject = fireteamObjectList[findSelectTeamIndex];

				if(selectTeamObject.ThisContainer.TryGetComponent<FireteamMovementAgent>(out var moveAnget))
				{
					moveAnget.OnMovementToAnchor(anchorIndex);
				}
			}
		}
	}
}
