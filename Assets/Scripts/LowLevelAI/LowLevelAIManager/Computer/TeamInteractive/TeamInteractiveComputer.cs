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
		private OdccQueryCollector valueCollector;

		private Dictionary<ITeamInteractiveValue, Dictionary<ITeamInteractiveValue, TeamInteractiveInfo>> computingList;

		[ShowInInspector, ReadOnly]
		private List<ITeamInteractiveValue> updateValueList;

		private Queue<Action> afterValueUpdate;
		private Queue<Action> afterComputeUpdate;
		public override void BaseAwake()
		{
			var computeValueQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<ITeamInteractiveValue>()
					.Build();

			afterValueUpdate = new Queue<Action>();
			afterComputeUpdate = new Queue<Action>();

			updateValueList = new List<ITeamInteractiveValue>();

			computingList = new Dictionary<ITeamInteractiveValue, Dictionary<ITeamInteractiveValue, TeamInteractiveInfo>>();

			valueCollector = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
				.CreateChangeListEvent(InitValueList, UpdateValueList)
				.CreateLooperEvent(nameof(ValueListUpdate), -1)
				.CallNext(ValueListUpdate)
				.CallNext(ValueListCompute)
				.GetCollector();

			//computeCollector = OdccQueryCollector.CreateQueryCollector(computeQuery, this)
			//	.CreateChangeListEvent(InitComputeList, UpdateComputeList)
			//	.CreateLooperEvent(nameof(ComputeListUpdate), -1)
			//	.JoinNext(_ValueListUpdate)
			//	.GetCollector();
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

		private void NewTarget(ITeamInteractiveValue actor, ITeamInteractiveValue target)
		{
			if(actor.ThisTeamData.IsEqualsTeam(target.ThisTeamData)) return;

			if(!computingList.TryGetValue(actor, out var inList))
			{
				inList = new Dictionary<ITeamInteractiveValue, TeamInteractiveInfo>();
				computingList.Add(actor, inList);
			}
			if(!inList.TryGetValue(target, out var info))
			{
				info = new TeamInteractiveInfo(actor, target);
				inList.Add(target, info);
			}
		}
		private void Compute(ITeamInteractiveValue actor, ITeamInteractiveValue target)
		{
			if(computingList.TryGetValue(actor, out var inList)
				&& inList.TryGetValue(target, out TeamInteractiveInfo info))
			{
				ITeamInteractiveValue actorValue = actor;
				ITeamInteractiveValue targetValue = target;

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

		public bool TryTeamTargetList(ITeamInteractiveValue actor, out Dictionary<ITeamInteractiveValue, TeamInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}

		public bool TryTeamTargetingInfo(ITeamInteractiveValue actor, ITeamInteractiveValue target, out TeamInteractiveInfo info)
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
