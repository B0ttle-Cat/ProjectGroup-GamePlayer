using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitDetector : ComponentBehaviour, IDetectorUpdate
	{
		public List<IUnitTarget> ResultList { get; set; }
		public List<IUnitTarget> ComputeList { get; set; }

		public async Awaitable StartCompute(List<IUnitTarget> checkList)
		{
			for(int i = 0 ; i < checkList.Count ; i++)
			{
				var check = checkList[i];

				//if(AbilityMath.IsDetecteOnVisibleTarget(this, check))
				//{
				//	ComputeList.Add(check);
				//	checkList.RemoveAt(i--);
				//}
			}
		}
		public async Awaitable EndedCompute()
		{

		}

		public float GetDetectorValue()
		{
			return 0;
		}
	}
}
