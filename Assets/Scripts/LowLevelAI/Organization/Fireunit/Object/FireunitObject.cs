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
				if(ThisContainer.TryGetParentObject<FireunitGrouping>(out var grouping))
				{
					if(grouping.ThisContainer.TryGetData<FireteamData>(out var fireteamData))
					{
						fireunitData.FactionIndex = fireteamData.FactionIndex;
						fireunitData.TeamIndex = fireteamData.TeamIndex;
						fireunitData.UnitIndex = ThisTransform.GetSiblingIndex();
						gameObject.name = $"{fireunitData.FactionIndex} : {fireunitData.TeamIndex} : Fireunit_{fireunitData.UnitIndex}";
					}
				}
				else
				{
					fireunitData.UnitIndex = ThisTransform.GetSiblingIndex();
					gameObject.name = $"None : None : Fireunit_{fireunitData.UnitIndex}";
				}
			}
		}
	}
}
