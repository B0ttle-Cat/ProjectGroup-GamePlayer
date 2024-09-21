using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class UnitTacticalCombatStateChanger : ComponentBehaviour, IUnitTacticalCombatStateChanger
	{
		bool IUnitTacticalCombatStateChanger.ShouldAttack(IUnitStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo)
		{
			return targetInfo.IsInActionRange && targetInfo.IsInAttackRange;
		}

		bool IUnitTacticalCombatStateChanger.ShouldMove(IUnitStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo)
		{
			return targetInfo.IsInActionRange && !targetInfo.IsInAttackRange;
		}

		IUnitTacticalCombatStateUpdate IUnitTacticalCombatStateChanger.ChangeNextState(IUnitStateValue.TacticalCombatStateType nextTacticalState)
		{
			return nextTacticalState switch {
				IUnitStateValue.TacticalCombatStateType.Attack => ThisContainer.AddComponent<UnitAttackCombatState>(),
				IUnitStateValue.TacticalCombatStateType.Move => ThisContainer.AddComponent<UnitMoveCombatState>(),
				IUnitStateValue.TacticalCombatStateType.None => ThisContainer.AddComponent<UnitNoneCombatState>(),
				_ => ThisContainer.AddComponent<UnitNoneCombatState>(),
			};
		}
	}
}
