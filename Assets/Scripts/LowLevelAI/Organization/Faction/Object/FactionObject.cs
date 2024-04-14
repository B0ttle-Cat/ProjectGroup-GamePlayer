using BC.GameBaseInterface;
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

			gameObject.name = $"{factionData.FactionIndex:00} Faction";
		}

		public void UpdateObjectName()
		{
			if(ThisContainer.TryGetData<FactionData>(out factionData))
			{
				gameObject.name = $"{factionData.FactionIndex:00} Faction";
			}
		}

		public IFactionData IFactionData => factionData as IFactionData;
	}
}
