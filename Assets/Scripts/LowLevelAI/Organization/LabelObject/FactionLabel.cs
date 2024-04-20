using BC.OdccBase;
using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FactionLabel : ObjectBehaviour, IGetFactionData
	{
		private FactionData factionData;
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<FactionData>(out factionData))
			{
				factionData = ThisContainer.AddData<FactionData>();
			}
			gameObject.name = $"{factionData.FactionIndex:00} Faction Members";
		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<FactionData>(out factionData))
			{
				factionData = ThisContainer.AddData<FactionData>();
			}
			gameObject.name = $"{factionData.FactionIndex:00} Faction Members";
		}
		public IFactionData IFactionData => factionData;
	}
}
