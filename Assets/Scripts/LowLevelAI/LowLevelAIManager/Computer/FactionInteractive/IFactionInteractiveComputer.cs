using System.Collections.Generic;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveComputer"/>
	/// <see cref="ITeamInteractiveComputer"/>
	/// <see cref="IUnitInteractiveComputer"/>
	/// </summary>
	public interface IFactionInteractiveComputer : IMemberInteractiveComputer
	{
		bool TryFactionTargetList(IFactionInteractiveValue actor, out Dictionary<IFactionInteractiveValue, FactionInteractiveInfo> targetToList);
		bool TryFactionTargetingInfo(IFactionInteractiveValue actor, IFactionInteractiveValue target, out FactionInteractiveInfo info);
	}
}
