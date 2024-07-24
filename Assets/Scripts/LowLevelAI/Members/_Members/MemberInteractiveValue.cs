using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public abstract class MemberInteractiveValue : ComponentBehaviour, IMemberInteractiveValue
	{
		public IFindCollectedMembers FindMembers { get; set; }
		public IMemberInteractiveComputer Computer { get; set; }

		public abstract void OnUpdateInit();
		public abstract void OnValueRefresh();
		public abstract void IsAfterValueUpdate();

		public bool TryMemberTargetList(out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList)
		{
			return Computer.TryMemberTargetList(this, out targetToList);
		}
		public bool TryMemberTargetInfo(IMemberInteractiveValue target, out MemberInteractiveInfo info)
		{
			return Computer.TryMemberTargetInfo(this, target, out info);
		}
	}
}
