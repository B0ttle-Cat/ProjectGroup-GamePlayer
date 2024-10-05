using System;
using System.Collections.Generic;

using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class FireunitInteractiveValue : IUnitInteractiveInterface//.Update : MonoBehaviour
	{
		void IUnitInteractiveInterface.BuffTimeUpdate()
		{
			ThisContainer.CallActionAllComponent<IBuffTimeUpdate>(call => {
				call.LifeTimeUpdate();
			});
		}
		void IUnitInteractiveInterface.UnitHealthUpdate()
		{
			var HealthPoint = PlayValueData.채력;
			if(HealthPoint.Value <= 0f)
			{
				StateValueData.IsRetire = true;
			}
			else
			{
				StateValueData.IsRetire = false;
			}

			PlayValueData.채력 = HealthPoint;
		}
		void IUnitInteractiveInterface.UnitPoseUpdate()
		{
			PoseValueData.ThisUnitPosition = ThisTransform.position;
			PoseValueData.ThisUnitPosition = ThisTransform.forward;
			PoseValueData.ThisUnitLookUP = ThisTransform.up;
		}
		void IUnitInteractiveInterface.VisualRangeUpdate()
		{
			var VisualRange = PlayValueData.시야거리;

			VisualRange.Value = MathF.Max(0f, VisualRange.Value);
			PlayValueData.시야거리 = VisualRange;
		}
		void IUnitInteractiveInterface.ActionRangeUpdate()
		{
			var ActionRange = PlayValueData.반응거리;
			var ChaseRange = PlayValueData.추적거리;

			ActionRange.Value = MathF.Max(0f, ActionRange.Value);
			ChaseRange.Value = MathF.Max(0f, ChaseRange.Value);
			PlayValueData.반응거리 = ActionRange;
			PlayValueData.추적거리 = ChaseRange;
		}
		void IUnitInteractiveInterface.AttackRangeUpdate()
		{
			var AttackRange = PlayValueData.공격러리;


			AttackRange.Value = MathF.Max(0f, AttackRange.Value);
			PlayValueData.공격러리 = AttackRange;
		}
		void IUnitInteractiveInterface.InActionRangeTargetList(List<UnitInteractiveInfo> inActionRangeTargetList)
		{
			InteractiveTargetData.UpdateList(inActionRangeTargetList);
		}
		void IUnitInteractiveInterface.TacticalCombatStateUpdate()
		{
			if(UpdateBattleStateChange(iUnitStateChanger, StateValueData.TacticalCombatState, out var nextTacticalState, out var targetInfo))
			{
				// CombatState 가 변경된 경우 들어옴
				iUnitStateUpdate.TacticalCombatStateExit();
				iUnitStateUpdate.DestroyThis();

				StateValueData.TacticalCombatState = nextTacticalState;
				iUnitStateUpdate = iUnitStateChanger.ChangeNextState(nextTacticalState);
				StateValueData.UnitTacticalCombatStateUpdate = iUnitStateUpdate;

				iUnitStateUpdate.TacticalCombatStateEnter();
			}
			UpdateUnitBattleState(iUnitStateUpdate, targetInfo);

			bool UpdateBattleStateChange(IUnitTacticalCombatStateChanger iUnitStateCheck, IUnitStateValue.TacticalCombatStateType prevTacticalState,
				out IUnitStateValue.TacticalCombatStateType nextTacticalState, out UnitInteractiveInfo interactiveInfo)
			{
				nextTacticalState = IUnitStateValue.TacticalCombatStateType.None;
				interactiveInfo = null;
				var targets = InteractiveTargetData.EnemyTargetList;
				if(tacticalStateChecker == null || tacticalStateChecker.Length == 0)
				{
					InitTacticalStateChecker(new Func<UnitInteractiveInfo, bool>[]
					{
							ShouldAttack,
							ShouldMove,
					});
				}
				foreach(var checker in tacticalStateChecker)
				{
					if(nextTacticalState != IUnitStateValue.TacticalCombatStateType.None) break;
					foreach(var targetInfo in targets)
					{
						if(checker(targetInfo))
						{
							nextTacticalState = DetermineNextState(checker);
							interactiveInfo = targetInfo;
							break;
						}
					}
				}

				void InitTacticalStateChecker(Func<UnitInteractiveInfo, bool>[] funcs)
				{
					tacticalStateChecker = funcs;
				}
				IUnitStateValue.TacticalCombatStateType DetermineNextState(Func<UnitInteractiveInfo, bool> checker)
				{
					if(checker == ShouldAttack) return IUnitStateValue.TacticalCombatStateType.Attack;
					else if(checker == ShouldMove) return IUnitStateValue.TacticalCombatStateType.Move;
					else return IUnitStateValue.TacticalCombatStateType.None;
				}

				bool ShouldAttack(UnitInteractiveInfo targetInfo)
				{
					return iUnitStateCheck.ShouldAttack(prevTacticalState, targetInfo);
				}
				bool ShouldMove(UnitInteractiveInfo targetInfo)
				{
					return iUnitStateCheck.ShouldMove(prevTacticalState, targetInfo);
				}
				return prevTacticalState != nextTacticalState;

			}
			void UpdateUnitBattleState(IUnitTacticalCombatStateUpdate unitTacticalState, UnitInteractiveInfo interactiveInfo)
			{
				unitTacticalState.TacticalCombatStateUpdate(interactiveInfo);
			}
		}
	}
}
