using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IFactionInteractiveComputer : IOdccComponent
	{
		bool TryFactionTargetList(IFactionInteractiveActor actor, out Dictionary<IFactionInteractiveTarget, FactionInteractiveInfo> targetToList);
		bool TryFactionTargetingInfo(IFactionInteractiveActor actor, IFactionInteractiveTarget target, out FactionInteractiveInfo info);
	}
	public interface IFactionInteractiveActor : IOdccComponent
	{
		public IFactionData ThisFactionData => ThisContainer.GetData<IFactionData>();
		public IFactionInteractiveValue ThisFactionComputeValue => ThisContainer.GetComponent<IFactionInteractiveValue>();
		void OnUpdateStartCompute(IFactionInteractiveComputer iComputer);
		void OnUpdateEndedCompute(IFactionInteractiveComputer iComputer);
	}
	public interface IFactionInteractiveTarget : IOdccComponent
	{
		public IFactionData ThisFactionData => ThisContainer.GetData<IFactionData>();
		public IFactionInteractiveValue ThisFactionComputeValue => ThisContainer.GetComponent<IFactionInteractiveValue>();
	}
}
