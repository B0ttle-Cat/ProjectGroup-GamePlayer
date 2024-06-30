using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamDetector : ComponentBehaviour, IDetectorUpdate
	{
		public List<IUnitInteractiveValue> ResultList { get; set; }
		public List<IUnitInteractiveValue> ComputeList { get; set; }
		public async Awaitable StartCompute(List<IUnitInteractiveValue> checkList)
		{
			if(ThisContainer.TryGetComponent<FactionMemberCollector>(out var collector))
			{
				await collector.AsyncForeach(async member => {
					if(member.ThisContainer.TryGetComponent<IDetectorUpdate>(out var detector))
					{
						await detector.OnStartCompute(checkList);
					}
				});
			}
		}
		public async Awaitable EndedCompute()
		{
			if(ThisContainer.TryGetComponent<FactionMemberCollector>(out var collector))
			{
				await collector.AsyncForeach(async member => {
					if(member.ThisContainer.TryGetComponent<IDetectorUpdate>(out var detector))
					{
						await detector.OnEndedCompute();
						ComputeList.AddRange(detector.ResultList);
					}
				});
			}
		}

		public float GetDetectorValue()
		{
			return 0;
		}
	}
}
