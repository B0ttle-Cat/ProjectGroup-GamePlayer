using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface ITeamInteractiveComputer : IOdccComponent
	{
		bool TryTeamTargetList(ITeamInteractiveActor actor, out Dictionary<ITeamInteractiveTarget, TeamInteractiveInfo> targetToList);
		bool TryTeamTargetingInfo(ITeamInteractiveActor actor, ITeamInteractiveTarget target, out TeamInteractiveInfo info);
	}
	public interface ITeamInteractiveActor : IOdccComponent
	{
		public IFireteamData ThisTeamData => ThisContainer.GetData<IFireteamData>();
		public ITeamInteractiveValue ThisTeamComputeValue => ThisContainer.GetComponent<ITeamInteractiveValue>();
		void OnUpdateStartCompute(ITeamInteractiveComputer iComputer);
		void OnUpdateEndedCompute(ITeamInteractiveComputer iComputer);
	}
	public interface ITeamInteractiveTarget : IOdccComponent
	{
		public IFireteamData ThisTeamData => ThisContainer.GetData<IFireteamData>();
		public ITeamInteractiveValue ThisTeamComputeValue => ThisContainer.GetComponent<ITeamInteractiveValue>();
	}
}
