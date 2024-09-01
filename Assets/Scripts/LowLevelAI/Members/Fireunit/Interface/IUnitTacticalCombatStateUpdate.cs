using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IUnitTacticalCombatStateUpdate : IOdccComponent
	{
		void TacticalCombatStateEnter();
		void TacticalCombatStateUpdate(UnitInteractiveInfo interactiveInfo);
		void TacticalCombatStateExit();
	}
}
