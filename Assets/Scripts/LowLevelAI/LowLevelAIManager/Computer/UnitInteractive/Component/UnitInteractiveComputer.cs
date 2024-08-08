using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class UnitInteractiveComputer : ComponentBehaviour, IUnitInteractiveComputer
	{
		public IFindCollectedMembers FindMembers { get; set; }

		private DiplomacyData diplomacyData;

		[SerializeField]
		private OdccQueryCollector valueListCompute;
		private Queue<Action> afterValueListUpdate;
		private DateTime limitStartTime = DateTime.Now;
		private double limitTime = 0.005;

		[Space]
		[ShowInInspector, ReadOnly]

		// Actor UniqueID : IUnitInteractiveValue
		private Dictionary<int, IUnitInteractiveValue> unitInteractiveValueList;
		// Actor UniqueID : Target UniqueID : IUnitInteractiveValue
		private Dictionary<int, Dictionary<int, UnitInteractiveInfo>> computingList;

		// Actor FactionID : Target UniqueID 
		private Dictionary<int, HashSet<int>> inRangeFactionVisual;

		public override void BaseAwake()
		{
			if(ThisContainer.TryGetComponent<IFindCollectedMembers>(out var findCollectedMembers))
			{
				FindMembers = findCollectedMembers;
			}

			var computeValueQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<IUnitInteractiveValue>()
					.Build();

			afterValueListUpdate = new Queue<Action>();

			unitInteractiveValueList = new Dictionary<int, IUnitInteractiveValue>();

			computingList = new Dictionary<int, Dictionary<int, UnitInteractiveInfo>>();

			ThisContainer.NextGetData<DiplomacyData>((_data) => {
				diplomacyData = _data;

				valueListCompute = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
					.CreateChangeListEvent(InitValueList, UpdateValueList)
					.CreateLooperEvent(nameof(valueListCompute), -2)
					.CallNext(LimitTimeUpdate)
					.CallNext(AfterValueListUpdate)

					.CallNext(BuffTimeUpdate)
					.CallNext(HealthPointUpdate)
					.CallNext(UnitPoseUpdate)
					.CallNext(VisualRangeUpdate)
					.CallNext(InRangeVisualUpdate)
					.CallNext(ActionRangeUpdate)
					.CallNext(InRangeActionUpdate)

					.GetCollector();
			});
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(valueListCompute != null)
			{
				valueListCompute.DeleteLooperEvent(nameof(valueListCompute));
				valueListCompute = null;
			}

			if(computingList != null)
			{
				computingList.Clear();
				computingList = null;
			}
			if(unitInteractiveValueList != null)
			{
				unitInteractiveValueList.Clear();
				unitInteractiveValueList = null;
			}

			if(afterValueListUpdate == null)
			{
				afterValueListUpdate.Clear();
				afterValueListUpdate = null;
			}
		}
		#region CreateChangeListEvent
		private void InitValueList(IEnumerable<ObjectBehaviour> enumerable)
		{
			unitInteractiveValueList.Clear();
			computingList.Clear();

			unitInteractiveValueList = enumerable.Select(item => item.ThisContainer.GetComponent<IUnitInteractiveValue>())
				.Where(item => item != null)
				.ToDictionary(item => item.UnitData.MemberUniqueID);

			int valueLength = unitInteractiveValueList.Count;
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				var inList = new Dictionary<int, UnitInteractiveInfo>();
				foreach(var targetItem in unitInteractiveValueList)
				{
					var target = targetItem.Value;

					if(actor.UniqueID == target.UniqueID) continue;

					var newInteractiveInfo = new UnitInteractiveInfo(actor, target, ComputeDiplomacyType(actor, target));
					inList.Add(target.UniqueID, newInteractiveInfo);
				}
				computingList.Add(actor.UniqueID, inList);

				actor.FindMembers = FindMembers;
				actor.Computer = this;
				actor.InteractiveInterface.InitValueUpdate();
			}
		}
		private void UpdateValueList(ObjectBehaviour behaviour, bool added)
		{
			if(afterValueListUpdate != null)
			{
				afterValueListUpdate.Enqueue(() => _UpdateList(behaviour, added));
			}
			else
			{
				_UpdateList(behaviour, added);
			}
			void _UpdateList(ObjectBehaviour behaviour, bool added)
			{
				if(added)
				{
					if(behaviour.ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var value))
					{
						int memberUniqueID = value.UnitData.MemberUniqueID;
						value.FindMembers = FindMembers;
						value.Computer = this;
						value.InteractiveInterface.InitValueUpdate();
						AddValueComputingList(value);
						unitInteractiveValueList.Add(memberUniqueID, value);
					}
				}
				else
				{
					if(behaviour.ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var value))
					{
						int memberUniqueID = value.UnitData.MemberUniqueID;
						value.InteractiveInterface.ReleaseValueUpdate();
						RemoveValueComputingList(value);
						unitInteractiveValueList.Remove(memberUniqueID);
					}
				}
			}

			void AddValueComputingList(IUnitInteractiveValue addValue)
			{
				int addUniqueID = addValue.UnitData.MemberUniqueID;
				var addList = new Dictionary<int, UnitInteractiveInfo>();
				foreach(var computingItem in computingList)
				{
					var actorUniqueID = computingItem.Key;
					var keyList = computingItem.Value;
					if(actorUniqueID == addValue.UnitData.MemberUniqueID) continue;
					if(!unitInteractiveValueList.TryGetValue(actorUniqueID, out var actorValue)) continue;

					var addInteractiveInfo = new UnitInteractiveInfo(actorValue, addValue, ComputeDiplomacyType(actorValue, addValue));
					var keyInteractiveInfo = new UnitInteractiveInfo(addValue, actorValue, ComputeDiplomacyType(addValue, actorValue));

					keyList.Add(addUniqueID, addInteractiveInfo);
					addList.Add(actorUniqueID, keyInteractiveInfo);
				}
				computingList.Add(addUniqueID, addList);
			}
			void RemoveValueComputingList(IUnitInteractiveValue deleteValue)
			{
				int deleteUniqueID = deleteValue.UnitData.MemberUniqueID;
				computingList.Remove(deleteUniqueID);
				foreach(var computingItem in computingList)
				{
					var dic = computingItem.Value;

					dic.Remove(deleteUniqueID);
				}
			}
		}
		#endregion
		#region CallNext
		async Awaitable AwaitTime()
		{
			var deltaTime = DateTime.Now - limitStartTime;
			if(deltaTime.TotalSeconds > limitTime)
			{
				await Awaitable.NextFrameAsync();
				limitStartTime = DateTime.Now;
			}
		}
		private void LimitTimeUpdate()
		{
			limitStartTime = DateTime.Now;
		}
		private void AfterValueListUpdate()
		{
			if(afterValueListUpdate != null)
			{
				Queue<Action> tempQueue = afterValueListUpdate;
				afterValueListUpdate.Clear();
				while(tempQueue.Count > 0)
				{
					tempQueue.Dequeue()?.Invoke();
				}
			}
			afterValueListUpdate = new Queue<Action>();

		}
		#endregion
		#region CallNext - Unit Value Update & Check
		private async Awaitable HealthPointUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.IsRetire) continue;
				actor.InteractiveInterface.UnitHealthUpdate();
				await AwaitTime();
			}
		}
		private async Awaitable BuffTimeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.IsRetire) continue;
				actor.InteractiveInterface.BuffTimeUpdate();
				await AwaitTime();
			}
		}
		private async Awaitable UnitPoseUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.IsRetire) continue;
				actor.InteractiveInterface.UnitPoseUpdate();
				await AwaitTime();
			}
		}
		private async Awaitable VisualRangeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.IsRetire) continue;
				actor.InteractiveInterface.VisualRangeUpdate();
				await AwaitTime();
			}
		}
		private async Awaitable InRangeVisualUpdate()
		{
			inRangeFactionVisual.Clear();
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				if(actor.IsRetire) continue;

				foreach(var targetItem in unitInteractiveValueList)
				{
					var target = targetItem.Value;

					if(actor.UniqueID == target.UniqueID) continue;
					if(target.IsRetire) continue;

					if(Compute(actor, target, out UnitInteractiveInfo unitInteractiveInfo))
					{
						int factionUniqueID = actor.UniqueID/10000;

						if(!inRangeFactionVisual.TryGetValue(factionUniqueID, out var list))
						{
							list = new HashSet<int>();
							inRangeFactionVisual.Add(factionUniqueID, list);
						}
						list.Add(target.UniqueID);
					}
				}
				await AwaitTime();
			}
			bool Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
			{
				info = null;
				if(computingList.TryGetValue(actor.UniqueID, out var inList)
					&& inList.TryGetValue(target.UniqueID, out info))
				{
					IUnitInteractiveValue actorValue = actor;
					IUnitInteractiveValue targetValue = target;

					///////////// 계산
					Vector3 direction = actorValue.PoseValueData.ThisUnitPosition - targetValue.PoseValueData.ThisUnitPosition;
					float distance = direction.magnitude;
					direction = direction.normalized;

					float radius = actorValue.PoseValueData.ThisUnitRadius + targetValue.PoseValueData.ThisUnitRadius;

					distance -= radius;
					if(distance < radius) distance = radius;

					///////////// 대입
					info.Distance = distance;
					info.Direction = direction;

					///////////// 계산
					bool inVisualRange = info.Distance <= actorValue.PlayValueData.VisualRange.Value;

					return inVisualRange;
				}
				return false;
			}
		}
		private async Awaitable ActionRangeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.IsRetire) continue;
				actor.InteractiveInterface.ActionRangeUpdate();
				await AwaitTime();
			}
		}
		private async Awaitable InRangeActionUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				if(actor.IsRetire) continue;

				int factionUniqueID = actor.UniqueID/10000;
				if(!inRangeFactionVisual.TryGetValue(factionUniqueID, out var inRangeFactionVisualList)) continue;

				var inActionRangeTargetList = new List<UnitInteractiveInfo>();
				foreach(int targetUniqueID in inRangeFactionVisualList)
				{
					if(actor.UniqueID == targetUniqueID) continue;
					if(unitInteractiveValueList.TryGetValue(targetUniqueID, out var target))
					{
						if(target.IsRetire) continue;

						if(Compute(actor, target, out UnitInteractiveInfo unitInteractiveInfo))
						{
							inActionRangeTargetList.Add(unitInteractiveInfo);
						}
					}
				}
				actor.InteractiveInterface.InActionRangeTargetList(inActionRangeTargetList);

				await AwaitTime();
			}
			bool Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
			{
				info = null;
				if(computingList.TryGetValue(actor.UniqueID, out var inList)
					&& inList.TryGetValue(target.UniqueID, out info))
				{
					IUnitInteractiveValue actorValue = actor;
					IUnitInteractiveValue targetValue = target;

					///////////// 계산
					bool isInActionStartRange = info.Distance <= actorValue.PlayValueData.ActionRange.Start;
					bool isInActionEndedRange = info.Distance <= actorValue.PlayValueData.ActionRange.Ended;
					bool isInActionRange = info.IsInActionRange;

					if(!isInActionRange && isInActionStartRange)
					{
						isInActionRange = true;
					}
					else if(isInActionRange && !isInActionEndedRange)
					{
						isInActionRange = false;
					}

					info.IsInActionRange = isInActionRange;
					info.IsInActionStartRange = isInActionStartRange;
					info.IsInActionEndedRange = isInActionEndedRange;

					return isInActionRange;
				}
				return false;
			}
		}
		#endregion
		#region IUnitInteractiveComputer
		private FactionDiplomacyType ComputeDiplomacyType(IUnitInteractiveValue actor, IUnitInteractiveValue target)
		{
			return diplomacyData.diplomacyTypeList.TryGetValue((actor.UnitData.FactionIndex, target.UnitData.FactionIndex), out var diplomacyType)
				? diplomacyType
				: FactionDiplomacyType.Neutral_Faction;
		}
		public bool TryUnitTargetList(IUnitInteractiveValue actor, out Dictionary<int, UnitInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor.UniqueID, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}
		public bool TryUnitTargetInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
		{
			if(computingList.TryGetValue(actor.UniqueID, out var inList) && inList.TryGetValue(target.UniqueID, out info))
			{
				return true;
			}
			info = null;
			return false;
		}
		#endregion
	}
}
