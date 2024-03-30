using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamObject : ObjectBehaviour
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
			FireteamData fireteamData = null;
			if(!ThisContainer.TryGetData<FireteamData>(out fireteamData))
			{
				fireteamData = ThisContainer.AddData<FireteamData>();
			}

			if(fireteamData != null && ThisContainer.TryGetParentObject<ObjectBehaviour>(out var @object, obj => obj.ThisContainer.TryGetData<FactionData>(out _)))
			{
				FactionData factionData = @object.ThisContainer.GetData<FactionData>();
				fireteamData.FactionIndex = factionData.FactionIndex;
				fireteamData.TeamIndex = ThisTransform.GetSiblingIndex();

				gameObject.name = $"{fireteamData.FactionIndex:00} | {fireteamData.TeamIndex:00} Team";
			}
			else
			{
				gameObject.name = $"__ | __ Team";
			}
		}
	}
}
