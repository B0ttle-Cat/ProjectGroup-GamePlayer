using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveComputer"/>
	/// <see cref="ITeamInteractiveComputer"/>
	/// <see cref="IUnitInteractiveComputer"/>
	/// </summary>
	public interface IFactionInteractiveComputer : IOdccComponent
	{
		bool TryFactionTargetList(IFactionInteractiveValue actor, out Dictionary<IFactionInteractiveValue, FactionInteractiveInfo> targetToList);
		bool TryFactionTargetingInfo(IFactionInteractiveValue actor, IFactionInteractiveValue target, out FactionInteractiveInfo info);
	}
}
