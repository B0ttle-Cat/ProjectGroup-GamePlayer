using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireunitObject : ObjectBehaviour
	{

		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FireunitData>(out FireunitData fireunitData))
			{
				fireunitData = ThisContainer.AddData<FireunitData>();
			}
			if(fireunitData != null)
			{
				if(ThisContainer.TryGetParentObject<TeamLabel>(out var teamLabel, (item) => item.IFireteamData != null))
				{
					var fireteamData = teamLabel.IFireteamData;
					fireunitData.FactionIndex = teamLabel.IFireteamData.FactionIndex;
					fireunitData.TeamIndex = teamLabel.IFireteamData.TeamIndex;
					fireunitData.UnitIndex = ThisTransform.GetSiblingIndex();
					gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} | {fireunitData.UnitIndex:00} Unit";
					//gameObject.name = $"{fireunitData.FactionIndex} : {fireunitData.TeamIndex} : Fireunit_{fireunitData.UnitIndex}";
				}
				else
				{
					gameObject.name = $"__ | __ | __ Unit";
				}
			}
		}
	}
}
