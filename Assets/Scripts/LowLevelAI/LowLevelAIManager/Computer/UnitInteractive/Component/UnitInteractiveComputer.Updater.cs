using System;
using System.Collections.Generic;

using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class UnitInteractiveComputer//Updater
	{
		#region List Update �� ���� ��������.
		/// <summary>
		/// ���� �ð� ��� �� ���� ���������� �Ѿ�� �񵿱� �޼���
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
		/// ���� �ð��� ������Ʈ�ϴ� �޼���
		/// </summary>
		private void LooperLimitTimeUpdate()
		{
			limitStartTime = DateTime.Now;
		}

		/// <summary>
		/// afterValueListUpdate ť�� ó���ϴ� �޼���
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

		#region ������� ���ֿ� ���� ������Ʈ ����
		/// <summary>
		/// ������ ü�� ������ ������Ʈ�ϴ� �񵿱� �޼���
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
		/// ������ ���� �ð��� ������Ʈ�ϴ� �񵿱� �޼���
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
		/// ������ Transform�� ������Ʈ�ϴ� �񵿱� �޼���
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
		/// ������ �þ� ������ ������Ʈ�ϴ� �񵿱� �޼���
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
		/// ������ �þ� ���� �ִ� Ÿ���� ������Ʈ�ϴ� �񵿱� �޼���
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
			/// ���� ���� ��ȣ�ۿ� ������ ����ϴ� �޼���
			/// </summary>
			/// <param name="actor">������ ����</param>
			/// <param name="target">Ÿ�� ����</param>
			/// <param name="info">���� ��ȣ�ۿ� ����</param>
			/// <returns>�þ� ���� ���� �ִ��� ����</returns>
			bool Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
			{
				info = null;
				if(computingList.TryGetValue(actor.MemberUniqueID, out var inList)
					&& inList.TryGetValue(target.MemberUniqueID, out info))
				{
					IUnitInteractiveValue actorValue = actor;
					IUnitInteractiveValue targetValue = target;

					///////////// ���
					Vector3 direction = actorValue.PoseValueData.ThisUnitPosition - targetValue.PoseValueData.ThisUnitPosition;
					float distance = direction.magnitude;
					direction = direction.normalized;

					float radius = actorValue.PoseValueData.ThisUnitRadius + targetValue.PoseValueData.ThisUnitRadius;

					distance -= radius;
					if(distance < radius) distance = radius;

					///////////// ����
					info.Distance = distance;
					info.Direction = direction;

					///////////// ���
					bool inVisualRange = info.Distance <= actorValue.PlayValueData.VisualRange.Value;

					return inVisualRange;
				}
				return false;
			}
		}

		/// <summary>
		/// ������ �ൿ ������ ������Ʈ�ϴ� �񵿱� �޼���
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
		/// ������ �ൿ ���� ���� �ִ� Ÿ���� ������Ʈ�ϴ� �񵿱� �޼���
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
			/// ���� ���� ��ȣ�ۿ� ������ ����ϴ� �޼���
			/// </summary>
			/// <param name="actor">������ ����</param>
			/// <param name="target">Ÿ�� ����</param>
			/// <param name="info">���� ��ȣ�ۿ� ����</param>
			/// <returns>�ൿ ���� ���� �ִ��� ����</returns>
			bool Compute(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
			{
				info = null;
				if(computingList.TryGetValue(actor.MemberUniqueID, out var inList)
					&& inList.TryGetValue(target.MemberUniqueID, out info))
				{
					IUnitInteractiveValue actorValue = actor;
					IUnitInteractiveValue targetValue = target;

					///////////// ���
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
		/// ������ ��Ÿ��� ������Ʈ�ϴ� �񵿱� �޼���
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
		/// ������ ��Ÿ� ���� ���� �ִ� Ÿ���� ������Ʈ�ϴ� �񵿱� �޼���
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
				///////////// ���
				bool isInAttackRange = false;
				if(info.IsInActionRange)
				{
					isInAttackRange = info.Distance <= actor.PlayValueData.AttackRange.Value;
				}
				info.IsInAttackRange = isInAttackRange;
			}
		}

		/// <summary><code>
		/// ������ ���� ���¸� ������Ʈ
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
