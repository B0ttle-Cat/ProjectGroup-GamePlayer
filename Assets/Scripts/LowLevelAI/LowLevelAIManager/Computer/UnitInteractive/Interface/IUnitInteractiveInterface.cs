using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{

	public interface IUnitInteractiveInterface : IOdccComponent
	{
		void InitValueUpdate();
		void ReleaseValueUpdate();
		void BuffTimeUpdate();
		void UnitHealthUpdate();
		void UnitPoseUpdate();
		void VisualRangeUpdate();
		void ActionRangeUpdate();
		void AttackRangeUpdate();
		void InActionRangeTargetList(List<UnitInteractiveInfo> inActionRangeTargetList);
		void TacticalStateUpdate();
	}
}
