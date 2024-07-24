using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IMemberInteractiveValue : IOdccComponent
	{
		IFindCollectedMembers FindMembers { get; set; }
		IMemberInteractiveComputer Computer { get; set; }

		void OnUpdateInit();
		void IsBeforeValueUpdate();
		void IsAfterValueUpdate();

		bool TryMemberTargetList(out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList);
		bool TryMemberTargetInfo(IMemberInteractiveValue target, out MemberInteractiveInfo info);
	}
}
