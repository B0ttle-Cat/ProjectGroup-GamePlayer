using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class UnitInteractiveComputer : ComponentBehaviour, IUnitInteractiveComputer
	{
		[SerializeField]
		private OdccQueryCollector computeCollector;
		[SerializeField]
		private OdccQueryCollector valueCollector;

		private Dictionary<IUnitInteractiveActor, Dictionary<IUnitInteractiveTarget, UnitInteractiveInfo>> computingList;
		[Space]
		[ShowInInspector, ReadOnly]
		private List<IUnitInteractiveActor> updateActorList;
		[ShowInInspector, ReadOnly]
		private List<IUnitInteractiveTarget> updateTargetList;
		[ShowInInspector, ReadOnly]
		private List<IUnitInteractiveValue> updateValueList;

		private Queue<Action> afterValueUpdate;
		private Queue<Action> afterComputeUpdate;

		public override void BaseAwake()
		{
			var computeQuery = QuerySystemBuilder.CreateQuery()
					.WithAny<IUnitInteractiveActor, IUnitInteractiveTarget>()
					.WithAll<IUnitInteractiveValue>()
					.Build();

			var computeValueQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<IUnitInteractiveValue>()
					.Build();

			afterValueUpdate = new Queue<Action>();
			afterComputeUpdate = new Queue<Action>();

			updateActorList = new List<IUnitInteractiveActor>();
			updateTargetList = new List<IUnitInteractiveTarget>();
			updateValueList = new List<IUnitInteractiveValue>();

			computingList = new Dictionary<IUnitInteractiveActor, Dictionary<IUnitInteractiveTarget, UnitInteractiveInfo>>();

			valueCollector  = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
				.CreateChangeListEvent(InitValueList, UpdateValueList)
				.CreateActionEvent(nameof(ValueListUpdate), out var _ValueListUpdate)
				.GetCollector();

			computeCollector = OdccQueryCollector.CreateQueryCollector(computeQuery, this)
				.CreateChangeListEvent(InitComputeList, UpdateComputeList)
				.CreateLooperEvent(nameof(ComputeListUpdate), -1)
				.JoinNext(_ValueListUpdate)
				.CallNext(ComputeListUpdate)
				.GetCollector();
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(computeCollector != null)
			{
				computeCollector.DeleteLooperEvent(nameof(ComputeListUpdate));
				computeCollector = null;
			}
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

			if(updateActorList != null)
			{
				updateActorList.Clear();
				updateActorList = null;
			}
			if(updateTargetList != null)
			{
				updateTargetList.Clear();
				updateTargetList = null;
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
		private void InitComputeList(IEnumerable<ObjectBehaviour> enumerable)
		{
			afterComputeUpdate.Enqueue(() => _InitList(enumerable));
			void _InitList(IEnumerable<ObjectBehaviour> enumerable)
			{
				updateActorList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<IUnitInteractiveActor>()));
				updateTargetList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<IUnitInteractiveTarget>()));
				int actorLength = updateActorList.Count;
				int targetLength = updateTargetList.Count;
				for(int i = 0 ; i < actorLength ; i++)
				{
					for(int ii = 0 ; ii < targetLength ; ii++)
					{
						NewTarget(updateActorList[i], updateTargetList[ii]);
					}
				}
			}
		}
		private void UpdateComputeList(ObjectBehaviour behaviour, bool added)
		{
			afterComputeUpdate.Enqueue(() => _UpdateList(behaviour, added));
			void _UpdateList(ObjectBehaviour behaviour, bool added)
			{
				if(added)
				{
					var addedActorList = behaviour.ThisContainer.GetAllComponent<IUnitInteractiveActor>();
					var addedTargetList = behaviour.ThisContainer.GetAllComponent<IUnitInteractiveTarget>();

					updateTargetList.AddRange(addedTargetList);

					int actorLength = addedActorList.Length;
					int targetLength = updateTargetList.Count;
					for(int i = 0 ; i < actorLength ; i++)
					{
						for(int ii = 0 ; ii < targetLength ; ii++)
						{
							var actor = addedActorList[i];
							var target = updateTargetList[ii];
							NewTarget(actor, target);
						}
					}

					actorLength = updateActorList.Count;
					targetLength = addedTargetList.Length;
					for(int i = 0 ; i < actorLength ; i++)
					{
						for(int ii = 0 ; ii < targetLength ; ii++)
						{
							var actor = updateActorList[i];
							var target = addedTargetList[ii];
							NewTarget(actor, target);
						}
					}

					updateActorList.AddRange(addedActorList);
				}
				else
				{
					var deleteActorList = behaviour.ThisContainer.GetAllComponent<IUnitInteractiveActor>();
					var deleteTargetList = behaviour.ThisContainer.GetAllComponent<IUnitInteractiveTarget>();
					int deleteActorLength = deleteActorList.Length;
					int deleteTargetLength = deleteTargetList.Length;
					Action deleteAction = null;

					for(int i = 0 ; i < deleteActorLength ; i++)
					{
						updateActorList.Remove(deleteActorList[i]);
					}
					for(int i = 0 ; i < deleteTargetLength ; i++)
					{
						updateTargetList.Remove(deleteTargetList[i]);
					}

					foreach(var actorItem in computingList)
					{
						var actorKey = actorItem.Key;
						var findActor = deleteActorList.FirstOrDefault(actor => actor.ThisUnitData.IsEqualsUnit(actorKey.ThisUnitData));

						if(findActor != null)
						{
							deleteAction += () => {
								computingList[actorKey].Clear();
								computingList.Remove(actorKey);
							};
							continue;
						}

						var targetList = computingList[actorKey];
						foreach(var targetItem in targetList)
						{
							var targetKey = targetItem.Key;
							var findTarget = deleteTargetList.FirstOrDefault(actor => actor.ThisUnitData.IsEqualsUnit(targetKey.ThisUnitData));

							if(findTarget != null)
							{
								deleteAction += () => {
									computingList[actorKey][targetKey] = null;
									computingList[actorKey].Remove(targetKey);
								};
							}
						}
					}
					deleteAction?.Invoke();
				}
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
				value.OnUpdateValue();

				await AwaitTime();
			}
		}
		private async Awaitable ComputeListUpdate()
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


			int actorLength = updateActorList.Count;
			int targetLength = updateTargetList.Count;
			for(int i = 0 ; i < actorLength ; i++)
			{
				var actor = updateActorList[i];
				actor.OnUpdateStartCompute(this);
				for(int ii = 0 ; ii < targetLength ; ii++)
				{
					var target = updateTargetList[ii];
					Compute(actor, target);

					await AwaitTime();
				}
				actor.OnUpdateEndedCompute(this);
			}
		}

		private void NewTarget(IUnitInteractiveActor actor, IUnitInteractiveTarget target)
		{
			if(actor.ThisUnitData.IsEqualsUnit(target.ThisUnitData)) return;

			if(!computingList.TryGetValue(actor, out var inList))
			{
				inList = new Dictionary<IUnitInteractiveTarget, UnitInteractiveInfo>();
				computingList.Add(actor, inList);
			}
			if(!inList.TryGetValue(target, out var info))
			{
				info = new UnitInteractiveInfo(actor, target);
				inList.Add(target, info);
			}
		}
		private void Compute(IUnitInteractiveActor actor, IUnitInteractiveTarget target)
		{
			if(computingList.TryGetValue(actor, out var inList)
				&& inList.TryGetValue(target, out UnitInteractiveInfo info))
			{
				IUnitInteractiveValue actorValue = actor.ThisUnitComputeValue;
				IUnitInteractiveValue targetValue = target.ThisUnitComputeValue;

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

		public bool TryUnitTargetList(IUnitInteractiveActor actor, out Dictionary<IUnitInteractiveTarget, UnitInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}
		public bool TryUnitTargetingInfo(IUnitInteractiveActor actor, IUnitInteractiveTarget target, out UnitInteractiveInfo info)
		{
			if(computingList.TryGetValue(actor, out var inList) && inList.TryGetValue(target, out info))
			{
				return true;
			}
			info = null;
			return false;
		}
	}
}
