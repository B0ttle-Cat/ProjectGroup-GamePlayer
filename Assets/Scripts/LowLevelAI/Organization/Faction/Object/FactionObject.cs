using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IGetFactionData : IOdccObject
	{
		public IFactionData IFactionData { get; }
	}

	public class FactionObject : ObjectBehaviour, IGetFactionData
	{
		private FactionData factionData = null;
		public override void BaseValidate()
		{
			base.BaseValidate();
			factionData = null;
			if(!ThisContainer.TryGetData<FactionData>(out factionData))
			{
				factionData = ThisContainer.AddData<FactionData>();
			}

			if(!SetName())
			{
				gameObject.name = $"__ Faction __";
			}
			bool SetName()
			{
				if(factionData != null && ThisContainer.TryGetParentObject<ObjectBehaviour>(out var @object, obj => obj.ThisContainer.TryGetData<DiplomacyData>(out _)))
				{
					DiplomacyData diplomacyData = @object.ThisContainer.GetData<DiplomacyData>();
					if(diplomacyData.Table != null)
					{
						int findIndex = diplomacyData.Table.ItemList.FindIndex(item => item.FactionIndex == factionData.FactionIndex);
						if(findIndex>=0)
						{
							var findItem = diplomacyData.Table.ItemList[findIndex];
							gameObject.name = $"{findIndex:00} Faction {findItem.FactionName}";
							return true;
						}
					}
				}
				return false;
			}
		}

		public IFactionData IFactionData => factionData as IFactionData;
	}
}
