using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class FactionInteractiveValue : MemberInteractiveValue, IFactionInteractiveValue
	{
		public IFactionData ThisFactionData { get; set; }
		public IFactionDiplomacyData ThisDiplomacyData { get; set; }
		public FactionMemberCollector MemberCollector { get; set; }
		public override void OnUpdateInit()
		{
		}

		public override void OnValueRefresh()
		{
		}

		public override void IsAfterValueUpdate()
		{

		}
	}
}
