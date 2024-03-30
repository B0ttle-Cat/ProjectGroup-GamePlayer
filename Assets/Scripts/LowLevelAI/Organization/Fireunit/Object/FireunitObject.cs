using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IGetFireunitData : IOdccObject
	{
		public IFireunitData IFireunitData { get; }
		public IFireteamData IFireteamData { get; }
		public IFactionData IFactionData { get; }
	}
	public class FireunitObject : ObjectBehaviour, IGetFireunitData
	{
		private FireunitData fireunitData;
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FireunitData>(out fireunitData))
			{
				fireunitData = ThisContainer.AddData<FireunitData>();
			}
			if(fireunitData != null)
			{
				if(ThisContainer.TryGetParentObject<IGetFireteamData>(out var iGetFireteamData, (item) => item.IFireteamData != null))
				{
					IFireteamData fireteamData = iGetFireteamData.IFireteamData;
					fireunitData.FactionIndex = fireteamData.FactionIndex;
					fireunitData.TeamIndex = fireteamData.TeamIndex;
					fireunitData.UnitIndex = ThisTransform.GetSiblingIndex();
					gameObject.name = $"{fireunitData.FactionIndex:00} | {fireunitData.TeamIndex:00} | {fireunitData.UnitIndex:00} Unit";
				}
				else
				{
					gameObject.name = $"__ | __ | __ Unit";
				}
			}
		}

		public IFireunitData IFireunitData => fireunitData;
		public IFireteamData IFireteamData => fireunitData;
		public IFactionData IFactionData => fireunitData;
	}
}
