using BC.ODCC;

namespace BC.LowLevelAI
{
	public class TeamLabel : ObjectBehaviour, IGetFireteamData
	{
		private FireteamData fireteamData;
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FireteamData>(out fireteamData))
			{
				fireteamData = ThisContainer.AddData<FireteamData>();
			}
			gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team Members";
		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<FireteamData>(out fireteamData))
			{
				fireteamData = ThisContainer.AddData<FireteamData>();
			}
			gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team Members";
		}
		public IFireteamData IFireteamData => fireteamData;
		public IFactionData IFactionData => fireteamData;
	}
}
