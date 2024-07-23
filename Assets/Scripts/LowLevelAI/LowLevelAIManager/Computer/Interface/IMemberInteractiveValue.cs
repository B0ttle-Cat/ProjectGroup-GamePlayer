using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IMemberInteractiveValue : IOdccComponent
	{
		void OnUpdateInit();
		void OnValueRefresh();
	}
}
