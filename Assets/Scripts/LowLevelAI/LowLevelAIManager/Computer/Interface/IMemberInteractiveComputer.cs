using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IMemberInteractiveComputer : IOdccComponent
	{
		bool TryMemberTargetList(IMemberInteractiveValue actor, out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList);
		bool TryMemberTargetInfo(IMemberInteractiveValue actor, IMemberInteractiveValue target, out MemberInteractiveInfo info);
	}
}
