using BC.OdccBase;
using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IGetFireteamData : IOdccObject
	{
		public IFireteamData IFireteamData { get; }
		public IFactionData IFactionData { get; }
	}

	public class FireteamObject : ObjectBehaviour, IGetFireteamData
	{
		private FireteamData fireteamData = null;
		public override void BaseValidate()
		{
			base.BaseValidate();
			fireteamData = null;
			if(!ThisContainer.TryGetData<FireteamData>(out fireteamData))
			{
				fireteamData = ThisContainer.AddData<FireteamData>();
			}

			gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team";

		}

		public void UpdateObjectName()
		{
			if(ThisContainer.TryGetData<FireteamData>(out fireteamData))
			{
				gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team";
			}
		}


		public IFireteamData IFireteamData => fireteamData as IFireteamData;
		public IFactionData IFactionData => fireteamData as IFactionData;
	}
}
