using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class UnitInteractiveComputer : MemberInteractiveComputer, IUnitInteractiveComputer
	{
		private DiplomacyData diplomacyData;

		[SerializeField]
		private OdccQueryCollector valueListCompute;

		private Dictionary<IUnitInteractiveValue, Dictionary<IUnitInteractiveValue, UnitInteractiveInfo>> computingList;
		[Space]
		[ShowInInspector, ReadOnly]
		private List<IUnitInteractiveValue> updateValueList;

		private Queue<Action> afterValueListUpdate;

		DateTime limitStartTime = DateTime.Now;
		double limitTime = 0.005;

		public override void BaseValidate()
		{
			base.BaseValidate();
		}

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

			updateValueList = new List<IUnitInteractiveValue>();

			computingList = new Dictionary<IUnitInteractiveValue, Dictionary<IUnitInteractiveValue, UnitInteractiveInfo>>();

			ThisContainer.NextGetData<DiplomacyData>((_data) => {
				diplomacyData = _data;

				valueListCompute  = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
					.CreateChangeListEvent(InitValueList, UpdateValueList)
					.CreateLooperEvent(nameof(valueListCompute), -2)
					.CallNext(LimitTimeUpdate)
					.CallNext(BeforeValueUpdate)
					.CallNext(ValueListCompute)
					.CallNext(AfterValueUpdate)
					.GetCollector();
			});
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(valueListCompute != null)
			{
				valueListCompute.DeleteLooperEvent(nameof(BeforeValueUpdate));
				valueListCompute = null;
			}

			if(computingList != null)
			{
				computingList.Clear();
				computingList = null;
			}
			if(updateValueList != null)
			{
				updateValueList.Clear();
				updateValueList = null;
			}

			if(afterValueListUpdate == null)
			{
				afterValueListUpdate.Clear();
				afterValueListUpdate = null;
			}
		}
		private void InitValueList(IEnumerable<ObjectBehaviour> enumerable)
		{
			updateValueList.Clear();
			computingList.Clear();

			updateValueList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<IUnitInteractiveValue>()));

			int valueLength = updateValueList.Count;
			for(int i = 0 ; i < valueLength ; i++)
			{
				var actor = updateValueList[i];
				var inList = new Dictionary<IUnitInteractiveValue, UnitInteractiveInfo>();
				computingList.Add(actor, inList);

				for(int ii = 0 ; ii < valueLength ; ii++)
				{
					if(i == ii) continue;
					var target = updateValueList[ii];
					var newInteractiveInfo = new UnitInteractiveInfo(actor, target, ComputeDiplomacyType(actor, target));
					computingList[actor].Add(target, newInteractiveInfo);
				}

				actor.FindMembers = FindMembers;
				actor.Computer = this;
				actor.OnUpdateInit();
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
						value.FindMembers = FindMembers;
						value.Computer = this;
						value.OnUpdateInit();
						AddValueComputingList(value);
						updateValueList.Add(value);
					}
				}
				else
				{
					if(behaviour.ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var value))
					{
						RemoveValueComputingList(value);
						updateValueList.Remove(value);
					}
				}
			}

			void AddValueComputingList(IUnitInteractiveValue addValue)
			{
				var addList = new Dictionary<IUnitInteractiveValue, UnitInteractiveInfo>();
				foreach(var computingItem in computingList)
				{
					var keyValue = computingItem.Key;
					var keyList = computingItem.Value;
					if(keyValue.ThisUnitData.IsEqualsUnit(addValue.ThisUnitData)) continue;
					var addInteractiveInfo = new UnitInteractiveInfo(keyValue, addValue, ComputeDiplomacyType(keyValue, addValue));
					var keyInteractiveInfo = new UnitInteractiveInfo(addValue, keyValue, ComputeDiplomacyType(addValue, keyValue));

					keyList.Add(addValue, addInteractiveInfo);
					addList.Add(keyValue, keyInteractiveInfo);
				}
				computingList.Add(addValue, addList);
			}
			void RemoveValueComputingList(IUnitInteractiveValue deleteValue)
			{
				computingList.Remove(deleteValue);
				foreach(var computingItem in computingList)
				{
					var dic = computingItem.Value;

					dic.Remove(deleteValue);
				}
			}
		}

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
		private async Awaitable BeforeValueUpdate()
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

			int length = updateValueList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				updateValueList[i].IsBeforeValueUpdate();
				await AwaitTime();
			}
		}
		private async Awaitable ValueListCompute()
		{
			int valueLength = updateValueList.Count;
			for(int i = 0 ; i < valueLength ; i++)
			{
				var actor = updateValueList[i];
				for(int ii = 0 ; ii < valueLength ; ii++)
				{
					var target = updateValueList[ii];
					Compute(actor, target);
					await AwaitTime();
				}
			}
		}
		private async Awaitable AfterValueUpdate()
		{
			int length = updateValueList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				updateValueList[i].IsAfterValueUpdate();
				await AwaitTime();
			}

			if(afterValueListUpdate != null)
			{
				Queue<Action> tempQueue = afterValueListUpdate;
				afterValueListUpdate.Clear();
				while(tempQueue.Count > 0)
				{
					tempQueue.Dequeue()?.Invoke();
				}
			}
			afterValueListUpdate = null;
		}

		private void Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target)
		{
			if(computingList.TryGetValue(actor, out var inList)
				&& inList.TryGetValue(target, out UnitInteractiveInfo info))
			{
				IUnitInteractiveValue actorValue = actor;
				IUnitInteractiveValue targetValue = target;

				ComputePose();
				ComputeFlag();

				actor.OnComputeTarget(info);
				void ComputePose()
				{
					///////////// 계산
					Vector3 direction = actorValue.ThisUnitPosition - targetValue.ThisUnitPosition;
					float distance = direction.magnitude;
					direction = direction.normalized;

					float radius = actorValue.ThisUnitRadius + targetValue.ThisUnitRadius;

					distance -= radius;
					if(distance < radius) distance = radius;

					///////////// 대입
					info.Distance = distance;
					info.Direction = direction;
				}
				void ComputeFlag()
				{
					///////////// 계산
					bool inVisualRange = info.Distance <= actorValue.ThisVisualRange;
					bool inActionRange = inVisualRange && info.Distance <= actorValue.ThisActionRange;
					bool inAttackRange = inActionRange && info.Distance <= actorValue.ThisAttackRange;

					///////////// 대입
					info.IsInVisualRange = inVisualRange;
					info.IsInActionRange = inActionRange;
					info.IsInAttackRange = inAttackRange;
				}
			}
		}
		private FactionDiplomacyType ComputeDiplomacyType(IUnitInteractiveValue actor, IUnitInteractiveValue target)
		{
			return diplomacyData.diplomacyTypeList.TryGetValue((actor.ThisUnitData.FactionIndex, target.ThisUnitData.FactionIndex), out var diplomacyType)
				? diplomacyType
				: FactionDiplomacyType.Neutral_Faction;
		}

		public bool TryUnitTargetList(IUnitInteractiveValue actor, out Dictionary<IUnitInteractiveValue, UnitInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}
		public bool TryUnitTargetInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
		{
			if(computingList.TryGetValue(actor, out var inList) && inList.TryGetValue(target, out info))
			{
				return true;
			}
			info = null;
			return false;
		}
		public override bool TryMemberTargetList(IMemberInteractiveValue actor, out Dictionary<IMemberInteractiveValue, MemberInteractiveInfo> targetToList)
		{
			targetToList = null;
			if(actor is not IUnitInteractiveValue _actor) return false;
			if(!TryUnitTargetList(_actor, out var unitTargetToList)) return false;

			targetToList = unitTargetToList.ToDictionary(
			kvp => kvp.Key as IMemberInteractiveValue,
			kvp => kvp.Value as MemberInteractiveInfo);
			return targetToList != null;
		}
		public override bool TryMemberTargetInfo(IMemberInteractiveValue actor, IMemberInteractiveValue target, out MemberInteractiveInfo info)
		{
			info = null;
			if(actor is not IUnitInteractiveValue _actor) return false;
			if(target is not IUnitInteractiveValue _target) return false;
			if(!TryUnitTargetInfo(_actor, _target, out var unitTargetToList)) return false;

			if(unitTargetToList is not MemberInteractiveInfo memberInfo) return false;
			info = memberInfo;
			return info != null;
		}
	}
}
