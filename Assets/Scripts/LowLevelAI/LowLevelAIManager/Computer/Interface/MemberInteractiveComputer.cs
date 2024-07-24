using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public abstract class MemberInteractiveComputer : ComponentBehaviour, IMemberInteractiveComputer
	{
		public IFindCollectedMembers FindMembers { get; set; }
		public abstract bool TryMemberTargetList(IMemberInteractiveValue actor, out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList);
		public abstract bool TryMemberTargetInfo(IMemberInteractiveValue actor, IMemberInteractiveValue target, out MemberInteractiveInfo info);
	}
}
