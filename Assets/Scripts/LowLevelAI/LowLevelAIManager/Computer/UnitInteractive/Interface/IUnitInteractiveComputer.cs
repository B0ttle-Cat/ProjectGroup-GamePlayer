using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveComputer"/>
	/// <see cref="ITeamInteractiveComputer"/>
	/// <see cref="IUnitInteractiveComputer"/>
	/// </summary>
	public interface IUnitInteractiveComputer : IOdccComponent
	{
		IFindCollectedMembers FindMembers { get; set; }
		bool TryUnitTargetList(IUnitInteractiveValue actor, out Dictionary<int, UnitInteractiveInfo> targetToList);
		bool TryUnitTargetInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info);
	}
}
