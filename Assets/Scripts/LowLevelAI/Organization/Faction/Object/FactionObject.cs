using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FactionObject : ObjectBehaviour
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
			FactionData factionData = null;
			if(!ThisContainer.TryGetData<FactionData>(out factionData))
			{
				factionData = ThisContainer.AddData<FactionData>();
			}

			gameObject.name = $"Faction_None";
			if(factionData != null)
			{
				if(ThisContainer.TryGetParentObject<ObjectBehaviour>(out var @object, obj => obj.ThisContainer.TryGetData<DiplomacyData>(out _)))
				{
					DiplomacyData diplomacyData = @object.ThisContainer.GetData<DiplomacyData>();
					if(diplomacyData.Table != null)
					{
						int findIndex = diplomacyData.Table.ItemList.FindIndex(item => item.FactionIndex == factionData.FactionIndex);
						if(findIndex>=0)
						{
							var findItem = diplomacyData.Table.ItemList[findIndex];
							gameObject.name = $"Faction_{findItem.FactionName}";
						}
					}
				}
			}
		}
	}
}
