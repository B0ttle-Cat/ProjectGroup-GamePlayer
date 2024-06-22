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

			public void StartCompute()
			{
				target.StartCompute();
			}
			public void EndedCompute()
			{
				target.EndedCompute();
			}
		}

		private List<DetectorUpdateInfo> factionDetectorList;
		private List<DetectorUpdateInfo> fireteamDetectorList;
		private List<DetectorUpdateInfo> fireunitDetectorList;

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
						.CallNext(StartFireteamDetector)
							.CallNext(StartFireunitDetector)
							.CallNext(CompleteFireunitDetection)
						.CallNext(CompleteUpdateFireteamDetector)
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
			fireteamDetectorList = new List<DetectorUpdateInfo>();
			fireunitDetectorList = new List<DetectorUpdateInfo>();

			foreach(var behaviour in enumerable)
			{
				if(behaviour is FactionObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector1))
				{
					factionDetectorList.Add(new DetectorUpdateInfo(detector1));
				}
				else if(behaviour is FireteamObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector2))
				{
					fireteamDetectorList.Add(new DetectorUpdateInfo(detector2));
				}
				else if(behaviour is FireunitObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector3))
				{
					fireunitDetectorList.Add(new DetectorUpdateInfo(detector3));
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
				else if(behaviour is FireteamObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector2))
				{
					fireteamDetectorList.Add(new DetectorUpdateInfo(detector2));
				}
				else if(behaviour is FireunitObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector3))
				{
					fireunitDetectorList.Add(new DetectorUpdateInfo(detector3));
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
				else if(behaviour is FireteamObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector2))
				{
					int index = fireteamDetectorList.FindIndex(x => x.target == detector2);
					if(index>=0)
					{
						fireteamDetectorList.RemoveAt(index);
					}
				}
				else if(behaviour is FireunitObject && behaviour.TryGetComponent<IDetectorUpdate>(out var detector3))
				{
					int index = fireunitDetectorList.FindIndex(x => x.target == detector3);
					if(index>=0)
					{
						fireunitDetectorList.RemoveAt(index);
					}
				}
			}
		}



		private void StartFactionDetector()
		{
			int length = factionDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				factionDetectorList[i].StartCompute();
			}
		}
		private void StartFireteamDetector()
		{
			int length = fireteamDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				fireteamDetectorList[i].StartCompute();
			}
		}
		private async Awaitable StartFireunitDetector()
		{
			int length = fireunitDetectorList.Count;

			float waitDelta = 0.01f;
			float waitTime = Time.time;
			for(int i = 0 ; i < length ; i++)
			{
				var detector = fireunitDetectorList[i];
				detector.StartCompute();
				var nowTime = Time.time;
				if(nowTime - waitTime > waitDelta)
				{
					waitTime = nowTime;
					await Awaitable.NextFrameAsync();
				}
			}
		}
		private void CompleteUpdateFactionDetector()
		{
			int length = factionDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				factionDetectorList[i].EndedCompute();
			}
		}
		private void CompleteUpdateFireteamDetector()
		{
			int length = fireteamDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				fireteamDetectorList[i].EndedCompute();
			}
		}
		private void CompleteFireunitDetection()
		{
			int length = fireunitDetectorList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				fireunitDetectorList[i].EndedCompute();
			}
		}
	}
}
