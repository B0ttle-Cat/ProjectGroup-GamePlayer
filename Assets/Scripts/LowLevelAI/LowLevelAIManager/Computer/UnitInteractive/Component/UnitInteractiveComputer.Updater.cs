using System;
using System.Collections.Generic;

using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class UnitInteractiveComputer//Updater
	{
		#region List Update 를 위한 보조수단.
		/// <summary>
		/// 일정 시간 대기 후 다음 프레임으로 넘어가는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable AwaitTime()
		{
			var deltaTime = DateTime.Now - limitStartTime;
			if(deltaTime.TotalSeconds > limitTime)
			{
				await Awaitable.NextFrameAsync();
				limitStartTime = DateTime.Now;
			}
		}

		/// <summary>
		/// 제한 시간을 업데이트하는 메서드
		/// </summary>
		private void LooperLimitTimeUpdate()
		{
			limitStartTime = DateTime.Now;
		}

		/// <summary>
		/// afterValueListUpdate 큐를 처리하는 메서드
		/// </summary>
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

		#region 여기부터 유닛에 대한 업데이트 시작
		/// <summary>
		/// 유닛의 체력 정보를 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable HealthPointUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.StateValueData.IsRetire) continue;
				actor.InteractiveInterface.UnitHealthUpdate();
				await AwaitTime();
			}
		}

		/// <summary>
		/// 유닛의 버프 시간을 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable BuffTimeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.StateValueData.IsRetire) continue;
				actor.InteractiveInterface.BuffTimeUpdate();
				await AwaitTime();
			}
		}

		/// <summary>
		/// 유닛의 Transform을 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable UnitPoseUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.StateValueData.IsRetire) continue;
				actor.InteractiveInterface.UnitPoseUpdate();
				await AwaitTime();
			}
		}

		/// <summary>
		/// 유닛의 시야 범위를 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable VisualRangeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.StateValueData.IsRetire) continue;
				actor.InteractiveInterface.VisualRangeUpdate();
				await AwaitTime();
			}
		}

		/// <summary>
		/// 유닛의 시야 내에 있는 타겟을 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable InRangeVisualUpdate()
		{
			inRangeFactionVisual.Clear();
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				if(actor.StateValueData.IsRetire) continue;

				foreach(var targetItem in unitInteractiveValueList)
				{
					var target = targetItem.Value;

					if(actor.MemberUniqueID == target.MemberUniqueID) continue;
					if(target.StateValueData.IsRetire) continue;

					if(Compute(actor, target, out UnitInteractiveInfo unitInteractiveInfo))
					{
						int factionUniqueID = actor.FactionIndex;

						if(!inRangeFactionVisual.TryGetValue(factionUniqueID, out var list))
						{
							list = new HashSet<Vector3Int>();
							inRangeFactionVisual.Add(factionUniqueID, list);
						}
						list.Add(target.MemberUniqueID);
					}
				}
				await AwaitTime();
			}

			/// <summary>
			/// 유닛 간의 상호작용 정보를 계산하는 메서드
			/// </summary>
			/// <param name="actor">행위자 유닛</param>
			/// <param name="target">타겟 유닛</param>
			/// <param name="info">계산된 상호작용 정보</param>
			/// <returns>시야 범위 내에 있는지 여부</returns>
			bool Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
			{
				info = null;
				if(computingList.TryGetValue(actor.MemberUniqueID, out var inList)
					&& inList.TryGetValue(target.MemberUniqueID, out info))
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

		/// <summary>
		/// 유닛의 행동 범위를 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable ActionRangeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.StateValueData.IsRetire) continue;
				actor.InteractiveInterface.ActionRangeUpdate();
				await AwaitTime();
			}
		}

		/// <summary>
		/// 유닛의 행동 범위 내에 있는 타겟을 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable InRangeActionUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				if(actor.StateValueData.IsRetire) continue;

				int factionUniqueID = actor.FactionIndex;
				if(!inRangeFactionVisual.TryGetValue(factionUniqueID, out var inRangeFactionVisualList)) continue;

				var inActionRangeTargetList = new List<UnitInteractiveInfo>();
				foreach(Vector3Int targetUniqueID in inRangeFactionVisualList)
				{
					if(actor.MemberUniqueID == targetUniqueID) continue;
					if(unitInteractiveValueList.TryGetValue(targetUniqueID, out var target))
					{
						if(target.StateValueData.IsRetire) continue;

						if(Compute(actor, target, out UnitInteractiveInfo unitInteractiveInfo))
						{
							inActionRangeTargetList.Add(unitInteractiveInfo);
						}
					}
				}
				actor.InteractiveInterface.InActionRangeTargetList(inActionRangeTargetList);

				await AwaitTime();
			}

			/// <summary>
			/// 유닛 간의 상호작용 정보를 계산하는 메서드
			/// </summary>
			/// <param name="actor">행위자 유닛</param>
			/// <param name="target">타겟 유닛</param>
			/// <param name="info">계산된 상호작용 정보</param>
			/// <returns>행동 범위 내에 있는지 여부</returns>
			bool Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
			{
				info = null;
				if(computingList.TryGetValue(actor.MemberUniqueID, out var inList)
					&& inList.TryGetValue(target.MemberUniqueID, out info))
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

		/// <summary>
		/// 유닛의 사거리를 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable AttackRangeUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;

				if(actor.StateValueData.IsRetire) continue;
				actor.InteractiveInterface.AttackRangeUpdate();
				await AwaitTime();
			}
		}

		/// <summary>
		/// 유닛의 사거리 범위 내에 있는 타겟을 업데이트하는 비동기 메서드
		/// </summary>
		/// <returns></returns>
		private async Awaitable InRangeAttackUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var  actor = actorItem.Value;
				if(actor.StateValueData.IsRetire) continue;

				var list = actor.InteractiveTargetData.EnemyTargetList;
				int length = list.Length;
				for(int i = 0 ; i < length ; i++)
				{
					var info = list[i];
					var target = info.Target;
					if(actor.MemberUniqueID == target.MemberUniqueID) continue;
					if(target.StateValueData.IsRetire) continue;

					Compute(actor, target, info);
				}
				await AwaitTime();
			}

			void Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, UnitInteractiveInfo info)
			{
				///////////// 계산
				bool isInAttackRange = false;
				if(info.IsInActionRange)
				{
					isInAttackRange = info.Distance <= actor.PlayValueData.AttackRange.Value;
				}
				info.IsInAttackRange = isInAttackRange;
			}
		}

		/// <summary><code>
		/// 유닛의 전투 상태를 업데이트
		/// <see cref="OdccBase.IUnitStateValue.TacticalCombatStateType"/>
		/// </code></summary>
		/// <returns></returns>
		private async Awaitable TacticalCombatStateUpdate()
		{
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				if(actor.StateValueData.IsRetire) continue;

				actor.InteractiveInterface.TacticalCombatStateUpdate();
				await AwaitTime();
			}
		}



		#endregion
	}
}
