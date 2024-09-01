using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class UnitTacticalCombatStateChanger : ComponentBehaviour, IUnitTacticalCombatStateChanger
	{
		bool IUnitTacticalCombatStateChanger.ShouldAttack(ITacticalCombatStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo)
		{
			return targetInfo.IsInActionRange && targetInfo.IsInAttackRange;
		}

		bool IUnitTacticalCombatStateChanger.ShouldMove(ITacticalCombatStateValue.TacticalCombatStateType prevState, UnitInteractiveInfo targetInfo)
		{
			return targetInfo.IsInActionRange && !targetInfo.IsInAttackRange;
		}

		IUnitTacticalCombatStateUpdate IUnitTacticalCombatStateChanger.ChangeNextState(ITacticalCombatStateValue.TacticalCombatStateType nextTacticalState)
		{
			return nextTacticalState switch {
				ITacticalCombatStateValue.TacticalCombatStateType.Attack => ThisContainer.AddComponent<UnitAttackCombatState>(),
				ITacticalCombatStateValue.TacticalCombatStateType.Move => ThisContainer.AddComponent<UnitMoveCombatState>(),
				ITacticalCombatStateValue.TacticalCombatStateType.None => ThisContainer.AddComponent<UnitNoneCombatState>(),
				_ => ThisContainer.AddComponent<UnitNoneCombatState>(),
			};
		}
	}
}
