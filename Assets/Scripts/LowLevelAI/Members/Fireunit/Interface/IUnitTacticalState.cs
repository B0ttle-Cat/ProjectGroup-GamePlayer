using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IUnitTacticalState : IOdccComponent
	{
		void TacticalStateEnter();
		void TacticalStateUpdate();
		void TacticalStateExit();
	}
}
