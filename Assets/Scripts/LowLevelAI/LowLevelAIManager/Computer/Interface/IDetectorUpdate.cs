using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public interface IDetectorUpdate : IOdccComponent
	{
		public List<IUnitInteractiveValue> ResultList { get; set; }
		public List<IUnitInteractiveValue> ComputeList { get; set; }
		public async Awaitable OnStartCompute(List<IUnitInteractiveValue> checkList)
		{
			ComputeList = new List<IUnitInteractiveValue>();
			await StartCompute(checkList);
		}
		public async Awaitable OnEndedCompute()
		{
			await EndedCompute();
			ResultList = ComputeList;
		}
		public Awaitable StartCompute(List<IUnitInteractiveValue> checkList);
		public Awaitable EndedCompute();

		public float GetDetectorValue();
	}
}
