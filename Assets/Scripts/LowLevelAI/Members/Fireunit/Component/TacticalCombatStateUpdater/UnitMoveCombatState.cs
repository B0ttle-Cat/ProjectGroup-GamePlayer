using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class UnitMoveCombatState : ComponentBehaviour, IUnitTacticalCombatStateUpdate
	{
		IUnitIMovementAgent movementAgent;
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateEnter()
		{
			movementAgent = ThisContainer.GetComponent<IUnitIMovementAgent>();
		}
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateUpdate(UnitInteractiveInfo interactiveInfo)
		{
			if(movementAgent == null || interactiveInfo == null) return;
			movementAgent.InputMoveTarget(interactiveInfo.Target.PoseValueData.ThisUnitPosition);
		}
		void IUnitTacticalCombatStateUpdate.TacticalCombatStateExit()
		{
			if(movementAgent == null) return;
			movementAgent.InputMoveStop();
		}
	}
}
