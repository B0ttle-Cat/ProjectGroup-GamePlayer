using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IMemberInteractiveValue : IOdccComponent
	{
		IMemberInteractiveComputer Computer { get; set; }

		void OnUpdateInit();
		void OnValueRefresh();
		void IsAfterValueUpdate();

		bool TryMemberTargetList(out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList)
		{
			return Computer.TryMemberTargetList(this, out targetToList);
		}
		bool TryMemberTargetInfo(IMemberInteractiveValue target, out MemberInteractiveInfo info)
		{
			return Computer.TryMemberTargetInfo(this, target, out info);
		}
	}
}
