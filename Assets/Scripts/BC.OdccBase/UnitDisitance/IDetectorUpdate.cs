using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IDetectorUpdate : IOdccComponent
	{
		public List<IUnitTarget> ResultList { get; set; }
		public List<IUnitTarget> ComputeList { get; set; }
		public async Awaitable OnStartCompute(List<IUnitTarget> checkList)
		{
			ComputeList = new List<IUnitTarget>();
			await StartCompute(checkList);
		}
		public async Awaitable OnEndedCompute()
		{
			await EndedCompute();
			ResultList = ComputeList;
		}
		public Awaitable StartCompute(List<IUnitTarget> checkList);
		public Awaitable EndedCompute();

		public float GetDetectorValue();
	}
}
