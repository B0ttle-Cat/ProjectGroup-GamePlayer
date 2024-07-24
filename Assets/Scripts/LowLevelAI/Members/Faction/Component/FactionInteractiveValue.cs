using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class FactionInteractiveValue : MemberInteractiveValue, IFactionInteractiveValue
	{
		public IFactionData ThisFactionData { get; set; }
		public FactionMemberCollector MemberCollector { get; set; }
		public override void OnUpdateInit()
		{
		}
		public override void IsBeforeValueUpdate()
		{

		}
		public override void IsAfterValueUpdate()
		{

		}
	}
}
