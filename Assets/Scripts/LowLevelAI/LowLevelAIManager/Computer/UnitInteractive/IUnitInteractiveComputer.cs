using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveComputer"/>
	/// <see cref="ITeamInteractiveComputer"/>
	/// <see cref="IUnitInteractiveComputer"/>
	/// </summary>
	public interface IUnitInteractiveComputer : IOdccComponent
	{
		bool TryUnitTargetList(IUnitInteractiveValue actor, out Dictionary<IUnitInteractiveValue, UnitInteractiveInfo> targetToList);
		bool TryUnitTargetInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info);
	}
}
