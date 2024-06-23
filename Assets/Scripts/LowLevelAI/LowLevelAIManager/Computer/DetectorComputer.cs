using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class DetectorComputer : ComponentBehaviour
	{
		[SerializeField]
		private OdccQueryCollector detectorCollector;

		public class DetectorUpdateInfo
		{
			public IDetectorUpdate target;
			public DetectorUpdateInfo(IDetectorUpdate target)
			{
				this.target = target;
			}

			public async Awaitable StartCompute()
			{
				await target.OnStartCompute(null);
			}
			public async Awaitable EndedCompute()
			{
				await target.OnEndedCompute();
			}
		}

		private List<DetectorUpdateInfo> factionDetectorList;

		public List<IDetectorUpdate> updateThisFrameDetector;
		public override void BaseAwake()
		{
			var detectorQuery = QuerySystemBuilder.CreateQuery()
				.WithAll<IDetectorUpdate>()
				.Build();

			detectorCollector = OdccQueryCollector.CreateQueryCollector(detectorQuery, ThisObject)
				.CreateChangeListEvent(InitDetector, UpdateDetector)
				.CreateLooperEvent(nameof(IDetectorUpdate))
					.CallNext(StartFactionDetector)
					.CallNext(CompleteUpdateFactionDetector)
				.GetCollector();
		}

		public override void BaseDestroy()
		{
			if(detectorCollector != null)
			{
				detectorCollector.DeleteChangeListEvent(UpdateDetector);
				detectorCollector.DeleteLooperEvent(nameof(IDetectorUpdate));
				detectorCollector = null;
			}
		}

		private void InitDetector(IEnumerable<ObjectBehaviour> enumerable)
		{
			factionDetectorList = new List<DetectorUpdateInfo>();

			foreach(var behaviour in enumerable)
			{
				if(behaviour is FactionObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector1))
				{
					factionDetectorList.Add(new DetectorUpdateInfo(detector1));
				}
			}
		}
		private void UpdateDetector(ObjectBehaviour behaviour, bool added)
		{
			if(added)
			{
				if(behaviour is FactionObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector1))
				{
					factionDetectorList.Add(new DetectorUpdateInfo(detector1));
				}
			}
			else
			{
				if(behaviour is FactionObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector1))
				{
					int index = factionDetectorList.FindIndex(x => x.target == detector1);
					if(index>=0)
					{
						factionDetectorList.RemoveAt(index);
					}
				}
			}
		}

		private async Awaitable StartFactionDetector()
		{
			int length = factionDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				await factionDetectorList[i].StartCompute();
			}
		}
		private async Awaitable CompleteUpdateFactionDetector()
		{
			int length = factionDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				await factionDetectorList[i].EndedCompute();
			}
		}
	}
}
