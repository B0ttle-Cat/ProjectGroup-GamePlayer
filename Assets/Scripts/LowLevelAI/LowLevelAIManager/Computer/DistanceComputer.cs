using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class DistanceComputer : ComponentBehaviour
	{
		[SerializeField]
		private OdccQueryCollector unitTargetCollector;
		private List<IUnitTarget> updateTargetList;
		public record Targeting
		{
			public IUnitTarget from;
			public IUnitTarget to;

			public Targeting(IUnitTarget from, IUnitTarget to)
			{
				this.from=from;
				this.to=to;
			}
		}
		public struct TargetingInfo
		{
			public bool IsDefault;
			public float distance;
		}
		Dictionary<Targeting, TargetingInfo> targetingList;

		public override void BaseAwake()
		{
			var detectorQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<IUnitTarget>()
					.Build();

			unitTargetCollector = OdccQueryCollector.CreateQueryCollector(detectorQuery, this)
				.CreateChangeListEvent(InitList, UpdateList)
				.CreateLooperEvent(nameof(UpdateTargetingInfo), -1)
				.CallNext(UpdateTargetingInfo)
				.GetCollector();
		}

		private void InitList(IEnumerable<ObjectBehaviour> enumerable)
		{
			targetingList = new Dictionary<Targeting, TargetingInfo>();
			updateTargetList = enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<IUnitTarget>()).ToList();
			int length = updateTargetList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				for(int ii = 0 ; ii < length ; ii++)
				{
					if(i == ii) continue;
					var targeting = new Targeting(updateTargetList[i], updateTargetList[ii]);
					targetingList.Add(targeting, new TargetingInfo() {
						IsDefault = true
					});
				}
			}

		}
		private void UpdateList(ObjectBehaviour behaviour, bool added)
		{
			if(added)
			{
				var newList = behaviour.ThisContainer.GetAllComponent<IUnitTarget>();

				int length = newList.Length;
				for(int i = 0 ; i < length ; i++)
				{
					var newTarget = newList[i];
					int oldLength = updateTargetList.Count;
					for(int ii = 0 ; ii < oldLength ; ii++)
					{
						var oldTarget = updateTargetList[ii];
						var targeting = new Targeting(newTarget, oldTarget);
						targetingList.Add(targeting, new TargetingInfo() {
							IsDefault = true
						});
					}
					updateTargetList.Add(newList[i]);
				}
			}
			else
			{
				var newList = behaviour.ThisContainer.GetAllComponent<IUnitTarget>();
				List<Targeting> removeKeyList = new List<Targeting>();
				foreach(var item in targetingList)
				{
					Targeting key = item.Key;

					int length = newList.Length;
					for(int i = 0 ; i < length ; i++)
					{
						if(key.from == newList[i])
						{
							removeKeyList.Add(key);
							updateTargetList.Remove(key.from);
						}
						else if(key.to == newList[i])
						{
							removeKeyList.Add(key);
							updateTargetList.Remove(key.to);
						}
					}
				}
				{
					int length = removeKeyList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						targetingList.Remove(removeKeyList[i]);
					}
				}
			}
		}

		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(unitTargetCollector != null)
			{
				unitTargetCollector.DeleteLooperEvent(nameof(UpdateTargetingInfo));
				unitTargetCollector = null;
			}
		}

		private async Awaitable UpdateTargetingInfo()
		{
			int length = updateTargetList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				for(int ii = 0 ; ii < length ; ii++)
				{
					if(i == ii) continue;
					var targeting = new Targeting(updateTargetList[i], updateTargetList[ii]);
					if(targetingList.TryGetValue(targeting, out var info))
					{
						///////////// 계산
						float distance = Vector3.Distance(targeting.from.TargetingPosition, targeting.to.TargetingPosition);


						///////////// 대입
						info.distance = distance;


						///////////// 반영
						info.IsDefault = false;
						targetingList[targeting] = info;
					}
				}
			}
		}
	}
}
