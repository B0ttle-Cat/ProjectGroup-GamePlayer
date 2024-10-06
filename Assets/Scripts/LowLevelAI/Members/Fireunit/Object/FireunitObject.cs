using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IGetFireunitData : IOdccObject
	{
		public IFireunitData IFireunitData { get; }
		public IFireteamData IFireteamData { get; }
		public IFactionData IFactionData { get; }
	}
	public class FireunitObject : MemberObject, IGetFireunitData
	{
		private FireunitData fireunitData;
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FireunitData>(out fireunitData))
			{
				fireunitData = ThisContainer.AddData<FireunitData>();
			}

			if(IsNotEditingPrefab())
				gameObject.name = $"{fireunitData.FactionIndex:00} | {fireunitData.TeamIndex:00} | {fireunitData.UnitIndex:00} Unit";
		}
		public void UpdateObjectName()
		{
			if(ThisContainer.TryGetData<FireunitData>(out fireunitData))
			{
				if(IsNotEditingPrefab())
					gameObject.name = $"{fireunitData.FactionIndex:00} | {fireunitData.TeamIndex:00} | {fireunitData.UnitIndex:00} Unit";
			}
		}
		public IFireunitData IFireunitData => fireunitData;
		public IFireteamData IFireteamData => fireunitData;
		public IFactionData IFactionData => fireunitData;
	}
}
