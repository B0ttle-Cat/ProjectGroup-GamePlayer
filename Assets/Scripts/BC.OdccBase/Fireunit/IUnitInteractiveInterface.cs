using System.Collections.Generic;

using BC.ODCC;

namespace BC.OdccBase
{

	public partial interface IUnitInteractiveInterface : IOdccComponent
	{
		void BuffTimeUpdate();
		void UnitHealthUpdate();
		void UnitPoseUpdate();
		void VisualRangeUpdate();
		void ActionRangeUpdate();
		void AttackRangeUpdate();
		void InActionRangeTargetList(List<UnitInteractiveInfo> inActionRangeTargetList);
		void TacticalCombatStateUpdate();
	}
}
