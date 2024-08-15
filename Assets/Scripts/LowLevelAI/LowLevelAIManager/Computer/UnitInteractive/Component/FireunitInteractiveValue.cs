using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitInteractiveValue : ComponentBehaviour, IUnitInteractiveValue, IUnitInteractiveInterface
	{
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
			UnitData = ThisContainer.GetData<FireunitData>();
			if(ThisContainer.TryGetData<FireunitInteractiveTargetData>(out FireunitInteractiveTargetData interactiveTargetData))
			{
				InteractiveTargetData = interactiveTargetData;
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
			if(ThisContainer.TryGetData<FireunitPoseValue>(out var poseValueData))
			{
				PoseValueData = poseValueData;
				PoseValueData.UnitData = UnitData;
			}
			if(ThisContainer.TryGetData<FireunitStateValue>(out var stateValueData))
			{
				StateValueData = stateValueData;
				StateValueData.UnitData = UnitData;
			}
			InteractiveInterface = this;
			StateValueData.IsRetire = false;
		}
		void IUnitInteractiveInterface.InitValueUpdate()
		{
			StateValueData.IsRetire = false;
			InteractiveTargetData.Clear();
		}
		void IUnitInteractiveInterface.ReleaseValueUpdate()
		{
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
			InteractiveTargetData.NewRange(inActionRangeTargetList);
		}

		void IUnitInteractiveInterface.TacticalStateUpdate()
		{
			if(UpdateStateChange(StateValueData.TacticalState, out var nextTacticalState))
			{
				if(ThisContainer.TryGetComponent<IUnitTacticalState>(out var unitTacticalState))
				{
					unitTacticalState.TacticalStateExit();
					unitTacticalState.DestroyThis();
				}

				StateValueData.TacticalState = nextTacticalState;
				unitTacticalState = nextTacticalState switch {
					ITacticalStateValue.TacticalStateType.None => ThisContainer.AddComponent<UnitNoneState>(),
					ITacticalStateValue.TacticalStateType.Attack => ThisContainer.AddComponent<UnitAttackState>(),
					ITacticalStateValue.TacticalStateType.MovePos => ThisContainer.AddComponent<UnitMoveState>(),
					ITacticalStateValue.TacticalStateType.HoldPos => ThisContainer.AddComponent<UnitHoldState>(),
					_ => ThisContainer.AddComponent<UnitNoneState>(),
				};
				unitTacticalState.TacticalStateEnter();
				UpdateUnitState(unitTacticalState);
			}
			else
			{
				if(StateValueData.UnitTacticalComponent is IUnitTacticalState unitTacticalState)
				{
					UpdateUnitState(unitTacticalState);
				}
			}


			bool UpdateStateChange(ITacticalStateValue.TacticalStateType prevTacticalState, out ITacticalStateValue.TacticalStateType nextTacticalState)
			{
				// TODO :: 여기 좀더 다양하게 구분할 방법 찾아봐...

				nextTacticalState = prevTacticalState;
				var targets = InteractiveTargetData.EnemyTargetList;
				foreach(var targetInfo in targets)
				{
					if(ShouldAttack(targetInfo))
					{
						nextTacticalState = ITacticalStateValue.TacticalStateType.Attack;
						break;
					}
					else
					{
						continue;
					}
				}
				foreach(var targetInfo in targets)
				{
					if(ShouldAttack(targetInfo))
					{
						nextTacticalState = ITacticalStateValue.TacticalStateType.Attack;
						break;
					}
					else
					{
						continue;
					}
				}
				return prevTacticalState != nextTacticalState;

				bool ShouldAttack(UnitInteractiveInfo targetInfo)
				{
					// 공격 가능 조건 확인.
					return targetInfo.IsInActionRange;
				}
			}
			void UpdateUnitState(IUnitTacticalState unitTacticalState)
			{
				unitTacticalState.TacticalStateUpdate();
			}
		}
	}
}
