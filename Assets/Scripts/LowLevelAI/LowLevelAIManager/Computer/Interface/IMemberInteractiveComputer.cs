using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IMemberInteractiveComputer : IOdccComponent
	{
		IFindCollectedMembers FindMembers { get; set; }
		bool TryMemberTargetList(IMemberInteractiveValue actor, out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList);
		bool TryMemberTargetInfo(IMemberInteractiveValue actor, IMemberInteractiveValue target, out MemberInteractiveInfo info);
	}
}
