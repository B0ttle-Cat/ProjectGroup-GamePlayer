using System.Collections.Generic;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveComputer"/>
	/// <see cref="ITeamInteractiveComputer"/>
	/// <see cref="IUnitInteractiveComputer"/>
	/// </summary>
	public interface IUnitInteractiveComputer : IMemberInteractiveComputer
	{
		bool TryUnitTargetList(IUnitInteractiveValue actor, out Dictionary<IUnitInteractiveValue, UnitInteractiveInfo> targetToList);
		bool TryUnitTargetInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info);
	}
}
