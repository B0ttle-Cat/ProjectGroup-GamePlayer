using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IUnitTacticalCombatStateChanger : IOdccComponent
	{
		bool ShouldAttack(ITacticalCombatStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo);
		bool ShouldMove(ITacticalCombatStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo);
		IUnitTacticalCombatStateUpdate ChangeNextState(ITacticalCombatStateValue.TacticalCombatStateType nextTacticalState);
	}
}
