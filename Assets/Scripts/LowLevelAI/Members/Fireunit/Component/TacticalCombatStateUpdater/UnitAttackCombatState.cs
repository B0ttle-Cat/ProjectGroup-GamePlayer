using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class UnitAttackCombatState : ComponentBehaviour, IUnitTacticalCombatState
	{
		void IUnitTacticalCombatState.TacticalCombatStateEnter()
		{
		}
		bool IUnitTacticalCombatState.TacticalCombatStateCheck(ITacticalCombatStateValue.TacticalCombatStateType checkTo, UnitInteractiveInfo targetInfo)
		{
			return checkTo switch {
				ITacticalCombatStateValue.TacticalCombatStateType.Attack => ToAttack(),
				ITacticalCombatStateValue.TacticalCombatStateType.Move => ToMove(),
				_ => false,
			};
			bool ToAttack() => targetInfo.IsInActionRange && targetInfo.IsInAttackRange;
			bool ToMove() => targetInfo.IsInActionRange && !targetInfo.IsInAttackRange;
		}
		void IUnitTacticalCombatState.TacticalCombatStateUpdate()
		{
		}
		void IUnitTacticalCombatState.TacticalCombatStateExit()
		{
		}
	}
}
