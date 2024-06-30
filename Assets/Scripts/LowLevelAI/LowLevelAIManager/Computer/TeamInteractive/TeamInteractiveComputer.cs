using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class TeamInteractiveComputer : ComponentBehaviour, ITeamInteractiveComputer
	{
		[SerializeField]
		private OdccQueryCollector computeCollector;
		[SerializeField]
		private OdccQueryCollector valueCollector;

		private Dictionary<ITeamInteractiveActor, Dictionary<ITeamInteractiveTarget, TeamInteractiveInfo>> computingList;
		[Space]
		[ShowInInspector, ReadOnly]
		private List<ITeamInteractiveActor> updateActorList;
		[ShowInInspector, ReadOnly]
		private List<ITeamInteractiveTarget> updateTargetList;
		[ShowInInspector, ReadOnly]
		private List<ITeamInteractiveValue> updateValueList;

		private Queue<Action> afterValueUpdate;
		private Queue<Action> afterComputeUpdate;
		public override void BaseAwake()
		{
			var computeQuery = QuerySystemBuilder.CreateQuery()
					.WithAny<ITeamInteractiveActor, ITeamInteractiveTarget>()
					.WithAll<ITeamInteractiveValue>()
					.Build();

			var computeValueQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<ITeamInteractiveValue>()
					.Build();

			afterValueUpdate = new Queue<Action>();
			afterComputeUpdate = new Queue<Action>();

			updateActorList = new List<ITeamInteractiveActor>();
			updateTargetList = new List<ITeamInteractiveTarget>();
			updateValueList = new List<ITeamInteractiveValue>();

			computingList = new Dictionary<ITeamInteractiveActor, Dictionary<ITeamInteractiveTarget, TeamInteractiveInfo>>();

			valueCollector = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
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
				updateActorList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<ITeamInteractiveActor>()));
				updateTargetList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<ITeamInteractiveTarget>()));
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
					var addedActorList = behaviour.ThisContainer.GetAllComponent<ITeamInteractiveActor>();
					var addedTargetList = behaviour.ThisContainer.GetAllComponent<ITeamInteractiveTarget>();

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
					var deleteActorList = behaviour.ThisContainer.GetAllComponent<ITeamInteractiveActor>();
					var deleteTargetList = behaviour.ThisContainer.GetAllComponent<ITeamInteractiveTarget>();
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
						var findActor = deleteActorList.FirstOrDefault(actor => actor.ThisTeamData.IsEqualsTeam(actorKey.ThisTeamData));

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
							var findTarget = deleteTargetList.FirstOrDefault(actor => actor.ThisTeamData.IsEqualsTeam(targetKey.ThisTeamData));

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
				updateValueList.AddRange(enumerable.SelectMany(item => item.ThisContainer.GetAllComponent<ITeamInteractiveValue>()));
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
					var addValueList = behaviour.ThisContainer.GetAllComponent<ITeamInteractiveValue>();
					int valueLength = addValueList.Length;
					for(int i = 0 ; i < valueLength ; i++)
					{
						addValueList[i].OnUpdateInit();
					}
					updateValueList.AddRange(addValueList);
				}
				else
				{
					var addValueList = behaviour.ThisContainer.GetAllComponent<ITeamInteractiveValue>();
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

		private void NewTarget(ITeamInteractiveActor actor, ITeamInteractiveTarget target)
		{
			if(actor.ThisTeamData.IsEqualsTeam(target.ThisTeamData)) return;

			if(!computingList.TryGetValue(actor, out var inList))
			{
				inList = new Dictionary<ITeamInteractiveTarget, TeamInteractiveInfo>();
				computingList.Add(actor, inList);
			}
			if(!inList.TryGetValue(target, out var info))
			{
				info = new TeamInteractiveInfo(actor, target);
				inList.Add(target, info);
			}
		}
		private void Compute(ITeamInteractiveActor actor, ITeamInteractiveTarget target)
		{
			if(computingList.TryGetValue(actor, out var inList)
				&& inList.TryGetValue(target, out TeamInteractiveInfo info))
			{
				ITeamInteractiveValue actorValue = actor.ThisTeamComputeValue;
				ITeamInteractiveValue targetValue = target.ThisTeamComputeValue;

				ComputePose();

				void ComputePose()
				{
					///////////// 계산
					Vector3 direction = actorValue.ThisTeamPosition - targetValue.ThisTeamPosition;
					float distance = direction.magnitude;
					direction = direction.normalized;

					if(distance < 0f) distance = 0f;

					///////////// 대입
					info.Distance = distance;
					info.Direction = direction;
				}
			}
		}

		public bool TryTeamTargetList(ITeamInteractiveActor actor, out Dictionary<ITeamInteractiveTarget, TeamInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}

		public bool TryTeamTargetingInfo(ITeamInteractiveActor actor, ITeamInteractiveTarget target, out TeamInteractiveInfo info)
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
