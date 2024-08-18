using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IUnitTacticalCombatState : IOdccComponent
	{
		void TacticalCombatStateEnter();
		bool TacticalCombatStateCheck(ITacticalCombatStateValue.TacticalCombatStateType checkTo, UnitInteractiveInfo targetInfo);
		void TacticalCombatStateUpdate();
		void TacticalCombatStateExit();
	}
}
