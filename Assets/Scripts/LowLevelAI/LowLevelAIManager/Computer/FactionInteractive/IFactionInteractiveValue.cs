using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IFactionInteractiveValue : IMemberInteractiveValue
	{
		public IFactionData ThisFactionData { get; set; }
		public FactionMemberCollector MemberCollector { get; set; }
	}
}
