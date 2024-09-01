using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IGetFireteamData : IOdccObject
	{
		public IFireteamData IFireteamData { get; }
		public IFactionData IFactionData { get; }
	}

	public class FireteamObject : MemberObject, IGetFireteamData
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

			if(IsNotEditingPrefab())
				gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team";

		}

		public void UpdateObjectName()
		{
			if(ThisContainer.TryGetData<FireteamData>(out fireteamData))
			{
				if(IsNotEditingPrefab())
					gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team";
			}
		}


		public IFireteamData IFireteamData => fireteamData as IFireteamData;
		public IFactionData IFactionData => fireteamData as IFactionData;
	}
}
