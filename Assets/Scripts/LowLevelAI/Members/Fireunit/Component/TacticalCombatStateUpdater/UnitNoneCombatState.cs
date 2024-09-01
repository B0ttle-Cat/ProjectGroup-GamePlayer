using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class UnitNoneCombatState : ComponentBehaviour, IUnitTacticalCombatStateUpdate
	{
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateEnter()
		{
		}
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateUpdate(UnitInteractiveInfo interactiveInfo)
		{
		}
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateExit()
		{
		}
	}
}
