using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IUnitInteractiveComputer : IOdccComponent
	{
		bool TryUnitTargetList(IUnitInteractiveActor actor, out Dictionary<IUnitInteractiveTarget, UnitInteractiveInfo> targetToList);
		bool TryUnitTargetingInfo(IUnitInteractiveActor actor, IUnitInteractiveTarget target, out UnitInteractiveInfo info);
	}
	public interface IUnitInteractiveActor : IOdccComponent
	{
		public IFireunitData ThisUnitData => ThisContainer.GetData<IFireunitData>();
		public IUnitInteractiveValue ThisUnitComputeValue => ThisContainer.GetComponent<IUnitInteractiveValue>();
		void OnUpdateStartCompute(IUnitInteractiveComputer iComputer);
		void OnUpdateEndedCompute(IUnitInteractiveComputer iComputer);
	}
	public interface IUnitInteractiveTarget : IOdccComponent
	{
		public IFireunitData ThisUnitData => ThisContainer.GetData<IFireunitData>();
		public IUnitInteractiveValue ThisUnitComputeValue => ThisContainer.GetComponent<IUnitInteractiveValue>();
	}
}
