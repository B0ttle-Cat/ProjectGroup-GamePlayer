using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class FactionInteractiveValue : ComponentBehaviour, IFactionInteractiveValue
	{
		public IFactionData ThisFactionData { get; set; }
		public IFactionDiplomacyData ThisDiplomacyData { get; set; }

		public void OnUpdateInit()
		{
		}

		public void OnUpdateValue()
		{
		}
	}
}
