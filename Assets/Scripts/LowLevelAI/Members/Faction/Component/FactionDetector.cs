using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class FactionDetector : ComponentBehaviour, IDetectorUpdate
	{
		public List<IUnitTarget> ResultList { get; set; }
		public List<IUnitTarget> ComputeList { get; set; }

		OdccQueryCollector visibleTargetCollector;
		public override void BaseEnable()
		{
			base.BaseEnable();
			var query = QuerySystemBuilder.CreateQuery()
				.WithAll<FireunitObject, FireunitData, IUnitTarget>()
				.Build();

			visibleTargetCollector = OdccQueryCollector.CreateQueryCollector(query, this);
		}

		public override void BaseDisable()
		{
			base.BaseDisable();
			if(visibleTargetCollector != null)
			{
				visibleTargetCollector.RemoveLifeItem(this);
				visibleTargetCollector = null;
			}
		}

		public async Awaitable StartCompute(List<IUnitTarget> checkList)
		{
			if(visibleTargetCollector == null) return;
			checkList = visibleTargetCollector.GetQueryItems().SelectMany(item => item.ThisContainer.GetAllComponent<IUnitTarget>()).ToList();

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
