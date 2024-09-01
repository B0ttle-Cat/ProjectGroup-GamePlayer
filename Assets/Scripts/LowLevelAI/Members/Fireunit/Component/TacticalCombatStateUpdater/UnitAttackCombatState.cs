using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class UnitAttackCombatState : ComponentBehaviour, IUnitTacticalCombatStateUpdate
	{
		IUnitAttackerAgent attackerAgent;
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateEnter()
		{
			attackerAgent = ThisContainer.GetComponent<IUnitAttackerAgent>();
		}
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateUpdate(UnitInteractiveInfo interactiveInfo)
		{
			if(attackerAgent == null || interactiveInfo == null) return;

			attackerAgent.InputAttackTarget(interactiveInfo);
		}
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateExit()
		{
			attackerAgent.InputAttackStop();
		}
	}
}
