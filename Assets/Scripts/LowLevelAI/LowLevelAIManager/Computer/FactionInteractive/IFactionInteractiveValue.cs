using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IFactionInteractiveValue : IOdccComponent
	{
		public IFactionData ThisFactionData { get; set; }

		void OnUpdateInit();
		void OnUpdateValue();
	}
}
