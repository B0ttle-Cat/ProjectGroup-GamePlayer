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
		//[SerializeField]
		//private OdccQueryCollector computeCollector;
		[SerializeField]
		private OdccQueryCollector valueCollector;

		private Dictionary<IUnitInteractiveValue, Dictionary<IUnitInteractiveValue, UnitInteractiveInfo>> computingList;
		[Space]
		[ShowInInspector, ReadOnly]
		private List<IUnitInteractiveValue> updateValueList;

		private Queue<Action> afterValueUpdate;
		private Queue<Action> afterComputeUpdate;

		public override void BaseAwake()
		{
			if(ThisContainer.TryGetComponent<IFindCollectedMembers>(out var findCollectedMembers))
			{
				FindMembers = findCollectedMembers;
			}

			var computeValueQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<IUnitInteractiveValue>()
					.Build();

			afterValueUpdate = new Queue<Action>();
			afterComputeUpdate = new Queue<Action>();

			updateValueList = new List<IUnitInteractiveValue>();

			computingList = new Dictionary<IUnitInteractiveValue, Dictionary<IUnitInteractiveValue, UnitInteractiveInfo>>();

			valueCollector  = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
				.CreateChangeListEvent(InitValueList, UpdateValueList)
				.CreateLooperEvent(nameof(ValueListUpdate))
				.CallNext(ValueListUpdate)
				.CallNext(ValueListCompute)
				.CallNext(ValueListRefresh)
				.GetCollector();
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(valueCollector != null)
			{
				valueCollector.DeleteLooperEvent(nameof(ValueListUpdate));
				valueCollector = null;
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

			if(afterComputeUpdate != null)
			{
				afterComputeUpdate.Clear();
				afterComputeUpdate = null;
			}
			if(afterValueUpdate == null)
			{
				afterValueUpdate.Clear();
				afterValueUpdate = null;
			}
		}
		private void InitValueList(IEnumerable<ObjectBehaviour> enumerable)
		{
			afterValueUpdate.Enqueue(() => _InitList(enumerable));
			void _InitList(IEnumerable<ObjectBehaviour> enumerable)
			{
				updateValueList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<IUnitInteractiveValue>()));
				int valueLength = updateValueList.Count;
				for(int i = 0 ; i < valueLength ; i++)
				{
					updateValueList[i].FindMembers = FindMembers;
					updateValueList[i].Computer = this;
					updateValueList[i].OnUpdateInit();
				}
			}
		}
		private void UpdateValueList(ObjectBehaviour behaviour, bool added)
		{
			afterValueUpdate.Enqueue(() => _UpdateList(behaviour, added));
			void _UpdateList(ObjectBehaviour behaviour, bool added)
			{
				if(added)
				{
					var addValueList = behaviour.ThisContainer.GetAllComponent<IUnitInteractiveValue>();
					int valueLength = addValueList.Length;
					for(int i = 0 ; i < valueLength ; i++)
					{
						addValueList[i].FindMembers = FindMembers;
						addValueList[i].Computer = this;
						addValueList[i].OnUpdateInit();
					}
					updateValueList.AddRange(addValueList);
				}
				else
				{
					var addValueList = behaviour.ThisContainer.GetAllComponent<IUnitInteractiveValue>();
					int valueLength = addValueList.Length;
					for(int i = 0 ; i < valueLength ; i++)
					{
						updateValueList.Remove(addValueList[i]);
					}
				}
			}
		}

		private async Awaitable ValueListUpdate()
		{
			DateTime startTime = DateTime.Now;
			float limitTime = 0.005f;
			TimeSpan deltaTime;
			async Awaitable AwaitTime()
			{
				deltaTime = DateTime.Now - startTime;
				if(deltaTime.TotalMilliseconds > limitTime)
				{
					await Awaitable.NextFrameAsync();
					startTime = DateTime.Now;
				}
			}

			Queue<Action> tempQueue = afterValueUpdate;
			afterValueUpdate = new Queue<Action>();
			while(tempQueue.Count > 0)
			{
				var update = tempQueue.Dequeue();
				update?.Invoke();

				await AwaitTime();
			}


			int length = updateValueList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var value = updateValueList[i];
				value.OnValueRefresh();

				await AwaitTime();
			}
		}
		private async Awaitable ValueListCompute()
		{
			DateTime startTime = DateTime.Now;
			float limitTime = 0.005f;
			TimeSpan deltaTime;
			async Awaitable AwaitTime()
			{
				deltaTime = DateTime.Now - startTime;
				if(deltaTime.TotalMilliseconds > limitTime)
				{
					await Awaitable.NextFrameAsync();
					startTime = DateTime.Now;
				}
			}


			Queue<Action> tempQueue = afterComputeUpdate;
			afterComputeUpdate = new Queue<Action>();
			while(tempQueue.Count > 0)
			{
				var update = tempQueue.Dequeue();
				update?.Invoke();

				await AwaitTime();
			}


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

		private async Awaitable ValueListRefresh()
		{
			DateTime startTime = DateTime.Now;
			float limitTime = 0.005f;
			TimeSpan deltaTime;
			async Awaitable AwaitTime()
			{
				deltaTime = DateTime.Now - startTime;
				if(deltaTime.TotalMilliseconds > limitTime)
				{
					await Awaitable.NextFrameAsync();
					startTime = DateTime.Now;
				}
			}

			int length = updateValueList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var value = updateValueList[i];
				value.OnValueRefresh();

				await AwaitTime();
			}
		}

		private void Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target)
		{
			if(computingList.TryGetValue(actor, out var inList)
				&& inList.TryGetValue(target, out UnitInteractiveInfo info))
			{
				IUnitInteractiveValue actorValue = actor;
				IUnitInteractiveValue targetValue = target;

				ComputePose();
				ComputeRange();

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
				void ComputeRange()
				{
					///////////// 계산
					bool inVisualRange = info.Distance <= actorValue.ThisVisualRange;
					bool inActionRange = info.Distance <= actorValue.ThisActionRange;
					bool inAttackRange = info.Distance <= actorValue.ThisAttackRange;

					///////////// 대입
					info.IsInVisualRange = inVisualRange;
					info.IsInActionRange = inActionRange;
					info.IsInAttackRange = inAttackRange;
				}
			}
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
