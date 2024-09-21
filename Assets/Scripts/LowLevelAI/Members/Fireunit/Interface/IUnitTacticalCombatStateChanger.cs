using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IUnitTacticalCombatStateChanger : IOdccComponent
	{
		bool ShouldAttack(IUnitStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo);
		bool ShouldMove(IUnitStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo);
		IUnitTacticalCombatStateUpdate ChangeNextState(IUnitStateValue.TacticalCombatStateType nextTacticalState);
	}
}
