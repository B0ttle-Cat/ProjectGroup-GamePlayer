using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FactionInteractiveComputer : MemberInteractiveComputer, IFactionInteractiveComputer
	{
		[SerializeField]
		private OdccQueryCollector valueCollector;

		private Dictionary<IFactionInteractiveValue, Dictionary<IFactionInteractiveValue, FactionInteractiveInfo>> computingList;
		[ShowInInspector, ReadOnly]
		private List<IFactionInteractiveValue> updateValueList;

		private Queue<Action> afterValueUpdate;
		private Queue<Action> afterComputeUpdate;

		OdccQueryLooper OnFactionInteractive;
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
					.WithAll<IFactionInteractiveValue>()
					.Build();

			afterValueUpdate = new Queue<Action>();
			afterComputeUpdate = new Queue<Action>();

			updateValueList = new List<IFactionInteractiveValue>();

			computingList = new Dictionary<IFactionInteractiveValue, Dictionary<IFactionInteractiveValue, FactionInteractiveInfo>>();

			valueCollector = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
				.CreateChangeListEvent(InitValueList, UpdateValueList)
				.CreateLooperEvent(nameof(ValueListUpdate), -1)
				.CallNext(ValueListUpdate)
				.CallNext(ValueListCompute)
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
				updateValueList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<IFactionInteractiveValue>()));
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
					var addValueList = behaviour.ThisContainer.GetAllComponent<IFactionInteractiveValue>();
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
					var addValueList = behaviour.ThisContainer.GetAllComponent<IFactionInteractiveValue>();
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


			int actorLength = updateValueList.Count;
			for(int i = 0 ; i < actorLength ; i++)
			{
				var actor = updateValueList[i];
				for(int ii = 0 ; ii < actorLength ; ii++)
				{
					var target = updateValueList[ii];
					Compute(actor, target);

					await AwaitTime();
				}
			}
		}

		private void NewTarget(IFactionInteractiveValue actor, IFactionInteractiveValue target)
		{
			if(actor.ThisFactionData.IsEqualsFaction(target.ThisFactionData)) return;

			if(!computingList.TryGetValue(actor, out var inList))
			{
				inList = new Dictionary<IFactionInteractiveValue, FactionInteractiveInfo>();
				computingList.Add(actor, inList);
			}
			if(!inList.TryGetValue(target, out var info))
			{
				info = new FactionInteractiveInfo(actor, target);
				inList.Add(target, info);
			}
		}
		private void Compute(IFactionInteractiveValue actor, IFactionInteractiveValue target)
		{
			//if(computingList.TryGetValue(actor, out var inList)
			//	&& inList.TryGetValue(target, out FactionInteractiveInfo info))
			//{
			//	IFactionInteractiveValue actorValue = actor;
			//	IFactionInteractiveValue targetValue = target;
			//
			//	ComputeDiplomacy();
			//
			//	void ComputeDiplomacy()
			//	{
			//		FactionDiplomacyType actor2TargetDiplomacy = actorValue.ThisDiplomacyData.GetFactionDiplomacyType(targetValue.ThisFactionData);
			//		FactionDiplomacyType target2ActorDiplomacy = targetValue.ThisDiplomacyData.GetFactionDiplomacyType(actorValue.ThisFactionData);
			//		info.FactionDiplomacy = (actor2TargetDiplomacy, target2ActorDiplomacy) switch {
			//			(FactionDiplomacyType.Equal_Faction, _) => FactionDiplomacyType.Equal_Faction,
			//			(_, FactionDiplomacyType.Equal_Faction) => FactionDiplomacyType.Equal_Faction,
			//
			//			(FactionDiplomacyType.Enemy_Faction, _) => FactionDiplomacyType.Enemy_Faction,
			//			(_, FactionDiplomacyType.Enemy_Faction) => FactionDiplomacyType.Enemy_Faction,
			//
			//			(FactionDiplomacyType.Neutral_Faction, _) => FactionDiplomacyType.Neutral_Faction,
			//			(_, FactionDiplomacyType.Neutral_Faction) => FactionDiplomacyType.Neutral_Faction,
			//
			//			(FactionDiplomacyType.Alliance_Faction, FactionDiplomacyType.Alliance_Faction) => FactionDiplomacyType.Alliance_Faction,
			//
			//			_ => FactionDiplomacyType.Neutral_Faction,
			//		};
			//	}
			//}
		}

		public bool TryFactionTargetList(IFactionInteractiveValue actor, out Dictionary<IFactionInteractiveValue, FactionInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}
		public bool TryFactionTargetingInfo(IFactionInteractiveValue actor, IFactionInteractiveValue target, out FactionInteractiveInfo info)
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
			if(actor is not IFactionInteractiveValue _actor) return false;
			if(!TryFactionTargetList(_actor, out var unitTargetToList)) return false;

			targetToList = unitTargetToList.ToDictionary(
			kvp => kvp.Key as IMemberInteractiveValue,
			kvp => kvp.Value as MemberInteractiveInfo);
			return targetToList != null;
		}
		public override bool TryMemberTargetInfo(IMemberInteractiveValue actor, IMemberInteractiveValue target, out MemberInteractiveInfo info)
		{
			info = null;
			if(actor is not IFactionInteractiveValue _actor) return false;
			if(target is not IFactionInteractiveValue _target) return false;
			if(!TryFactionTargetingInfo(_actor, _target, out var unitTargetToList)) return false;

			if(unitTargetToList is not MemberInteractiveInfo memberInfo) return false;
			info = memberInfo;
			return info != null;
		}
	}
}
