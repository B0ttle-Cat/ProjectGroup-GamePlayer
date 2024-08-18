using System;
using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitInteractiveValue : ComponentBehaviour, IUnitInteractiveValue, IUnitInteractiveInterface
	{
		private Func<UnitInteractiveInfo, bool>[] tacticalStateChecker;

		// Data
		public IFireunitData UnitData { get; private set; }
		public IFindCollectedMembers FindMembers { get; set; }
		public IUnitInteractiveComputer Computer { get; set; }
		public IUnitPlayValue PlayValueData { get; private set; }
		public IUnitPoseValue PoseValueData { get; private set; }
		public IUnitStateValue StateValueData { get; private set; }
		public IUnitInteractiveInterface InteractiveInterface { get; private set; }
		public FireunitInteractiveTargetData InteractiveTargetData { get; private set; }


		public override void BaseAwake()
		{
			base.BaseAwake();
			InteractiveInterface = this;
			tacticalStateChecker = null;

			UnitData = ThisContainer.GetData<FireunitData>();
			if(ThisContainer.TryGetData<FireunitInteractiveTargetData>(out FireunitInteractiveTargetData interactiveTargetData))
			{
				InteractiveTargetData = interactiveTargetData;
				InteractiveTargetData.Clear();
			}
			else
			{
				InteractiveTargetData = ThisContainer.AddData<FireunitInteractiveTargetData>();
				InteractiveTargetData.Clear();
			}
			if(ThisContainer.TryGetData<FireunitPlayValue>(out var playValueData))
			{
				PlayValueData = playValueData;
				PlayValueData.UnitData = UnitData;
			}
			else
			{
				PlayValueData = ThisContainer.AddData<FireunitPlayValue>();
				PlayValueData.UnitData = UnitData;
			}
			if(ThisContainer.TryGetData<FireunitPoseValue>(out var poseValueData))
			{
				PoseValueData = poseValueData;
				PoseValueData.UnitData = UnitData;
			}
			else
			{
				PoseValueData = ThisContainer.AddData<FireunitPoseValue>();
				PoseValueData.UnitData = UnitData;
			}
			if(ThisContainer.TryGetData<FireunitStateValue>(out var stateValueData))
			{
				StateValueData = stateValueData;
				StateValueData.UnitData = UnitData;
				StateValueData.IsRetire = false;
			}
			else
			{
				StateValueData = ThisContainer.AddData<FireunitStateValue>();
				StateValueData.UnitData = UnitData;
				StateValueData.IsRetire = false;
			}
			if(ThisContainer.TryGetComponent<IUnitTacticalCombatState>(out var unitTacticalState))
			{
				StateValueData.UnitTacticalComponent = unitTacticalState;
			}
			else
			{
				StateValueData.UnitTacticalComponent = ThisContainer.AddComponent<UnitNoneCombatState>();
			}
		}

		public override void BaseDestroy()
		{
			InteractiveInterface = null;
			tacticalStateChecker = null;
			UnitData = null;

			if(InteractiveTargetData != null)
			{
				InteractiveTargetData.Dispose();
				InteractiveTargetData = null;
			}
			if(PlayValueData != null)
			{
				PlayValueData.Dispose();
				PlayValueData = null;
			}
			if(StateValueData != null)
			{
				StateValueData.Dispose();
				StateValueData = null;
			}
			if(InteractiveTargetData != null)
			{
				InteractiveTargetData.Dispose();
				InteractiveTargetData = null;
			}
		}

		void IUnitInteractiveInterface.BuffTimeUpdate()
		{
			ThisContainer.CallActionAllComponent<IBuffTimeUpdate>(call => {
				call.LifeTimeUpdate();
			});
		}
		void IUnitInteractiveInterface.UnitHealthUpdate()
		{
			var HealthPoint = PlayValueData.HealthPoint;
			if(HealthPoint.Value <= HealthPoint.Min)
			{
				StateValueData.IsRetire = true;
			}
			else
			{
				StateValueData.IsRetire = false;
			}

			PlayValueData.HealthPoint = HealthPoint;
		}
		void IUnitInteractiveInterface.UnitPoseUpdate()
		{
			PoseValueData.ThisUnitPosition = ThisTransform.position;
			PoseValueData.ThisUnitPosition = ThisTransform.forward;
			PoseValueData.ThisUnitLookUP = ThisTransform.up;
		}

		void IUnitInteractiveInterface.VisualRangeUpdate()
		{
			var VisualRange = PlayValueData.VisualRange;

			VisualRange.Value = 10f;
			VisualRange.Value = Mathf.Clamp(VisualRange.Value, VisualRange.Min, VisualRange.Max);

			PlayValueData.VisualRange = VisualRange;
		}
		void IUnitInteractiveInterface.ActionRangeUpdate()
		{
			var ActionRange = PlayValueData.ActionRange;

			ActionRange.Start = 10f;
			ActionRange.Ended = 12f;

			ActionRange.Start = Mathf.Clamp(ActionRange.Start, ActionRange.Min, ActionRange.Ended);
			ActionRange.Ended = Mathf.Clamp(ActionRange.Ended, ActionRange.Start, ActionRange.Max);

			PlayValueData.ActionRange = ActionRange;
		}
		void IUnitInteractiveInterface.AttackRangeUpdate()
		{
			var AttackRange = PlayValueData.AttackRange;

			AttackRange.Value = 10f;
			AttackRange.Value = Mathf.Clamp(AttackRange.Value, AttackRange.Min, AttackRange.Max);

			PlayValueData.AttackRange = AttackRange;
		}
		void IUnitInteractiveInterface.InActionRangeTargetList(List<UnitInteractiveInfo> inActionRangeTargetList)
		{
			InteractiveTargetData.UpdateList(inActionRangeTargetList);
		}

		void IUnitInteractiveInterface.TacticalCombatStateUpdate()
		{
			if(UpdateBattleStateChange(StateValueData.TacticalCombatState, out var nextTacticalState))
			{
				if(ThisContainer.TryGetComponent<IUnitTacticalCombatState>(out var unitTacticalState))
				{
					unitTacticalState.TacticalCombatStateExit();
					unitTacticalState.DestroyThis();
				}

				StateValueData.TacticalCombatState = nextTacticalState;
				unitTacticalState = nextTacticalState switch {
					ITacticalCombatStateValue.TacticalCombatStateType.None => ThisContainer.AddComponent<UnitNoneCombatState>(),
					ITacticalCombatStateValue.TacticalCombatStateType.Attack => ThisContainer.AddComponent<UnitAttackCombatState>(),
					ITacticalCombatStateValue.TacticalCombatStateType.Move => ThisContainer.AddComponent<UnitMoveCombatState>(),
					_ => ThisContainer.AddComponent<UnitNoneCombatState>(),
				};

				StateValueData.UnitTacticalComponent = unitTacticalState;

				unitTacticalState.TacticalCombatStateEnter();
				UpdateUnitBattleState(unitTacticalState);
			}
			else
			{
				if(StateValueData.UnitTacticalComponent is IUnitTacticalCombatState unitTacticalState)
				{
					UpdateUnitBattleState(unitTacticalState);
				}
			}


			bool UpdateBattleStateChange(ITacticalCombatStateValue.TacticalCombatStateType prevTacticalState, out ITacticalCombatStateValue.TacticalCombatStateType nextTacticalState)
			{
				var unitTacticalState = StateValueData.UnitTacticalComponent as IUnitTacticalCombatState;


				nextTacticalState = ITacticalCombatStateValue.TacticalCombatStateType.None;
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
					if(nextTacticalState != ITacticalCombatStateValue.TacticalCombatStateType.None) break;
					foreach(var targetInfo in targets)
					{
						if(checker(targetInfo))
						{
							nextTacticalState = DetermineNextState(checker);
							break;
						}
					}
				}

				return prevTacticalState != nextTacticalState;

				void InitTacticalStateChecker(Func<UnitInteractiveInfo, bool>[] funcs)
				{
					tacticalStateChecker = funcs;
				}
				ITacticalCombatStateValue.TacticalCombatStateType DetermineNextState(Func<UnitInteractiveInfo, bool> checker)
				{
					if(checker == ShouldAttack) return ITacticalCombatStateValue.TacticalCombatStateType.Attack;
					else if(checker == ShouldMove) return ITacticalCombatStateValue.TacticalCombatStateType.Move;
					else return ITacticalCombatStateValue.TacticalCombatStateType.None;
				}

				bool ShouldAttack(UnitInteractiveInfo targetInfo)
				{
					return unitTacticalState != null && unitTacticalState.TacticalCombatStateCheck(ITacticalCombatStateValue.TacticalCombatStateType.Attack, targetInfo);
				}
				bool ShouldMove(UnitInteractiveInfo targetInfo)
				{
					return unitTacticalState != null  && unitTacticalState.TacticalCombatStateCheck(ITacticalCombatStateValue.TacticalCombatStateType.Move, targetInfo);
				}
			}
			void UpdateUnitBattleState(IUnitTacticalCombatState unitTacticalState)
			{

			}
		}
	}
}
