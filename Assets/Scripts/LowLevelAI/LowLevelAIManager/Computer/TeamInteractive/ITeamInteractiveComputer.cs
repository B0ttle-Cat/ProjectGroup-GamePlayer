using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveComputer"/>
	/// <see cref="ITeamInteractiveComputer"/>
	/// <see cref="IUnitInteractiveComputer"/>
	/// </summary>
	public interface ITeamInteractiveComputer : IOdccComponent
	{
		bool TryTeamTargetList(ITeamInteractiveValue actor, out Dictionary<ITeamInteractiveValue, TeamInteractiveInfo> targetToList);
		bool TryTeamTargetingInfo(ITeamInteractiveValue actor, ITeamInteractiveValue target, out TeamInteractiveInfo info);
	}
}
