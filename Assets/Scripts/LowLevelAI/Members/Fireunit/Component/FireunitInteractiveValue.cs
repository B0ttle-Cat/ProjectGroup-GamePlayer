using System;

using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitInteractiveValue : MemberInteractiveValue, IUnitInteractiveValue
	{
		// Data
		public IFireunitData ThisUnitData { get; set; }

		/// Value
		public float ThisUnitRadius { get; set; }
		public Vector3 ThisUnitPosition { get; set; }
		public Vector3 ThisUnitLookAt { get; set; }
		public Vector3 ThisUnitLookUP { get; set; }

		public float ThisVisualRange { get; set; }
		public float ThisActionRange { get; set; }
		public float ThisAttackRange { get; set; }

		public float ThisAntiVisualRange { get; set; }
		public float ThisAntiAttackRange { get; set; }

		public float MinVisualRange { get; set; }
		public float MaxVisualRange { get; set; }
		public float MinAttackRange { get; set; }
		public float MaxAttackRange { get; set; }

		/// Flag
		public bool IsInVisualRange { get; set; }   // 시야 범위 안에 있음
		public bool IsInActionRange { get; set; }   // 대응 범위 안에 있음
		public bool IsInAttackRange { get; set; }   // 공격 범위 안에 있음
		private bool TempIsInVisualRange;           // 시야 범위 안에 있음
		private bool TempIsInActionRange;           // 대응 범위 안에 있음
		private bool TempIsInAttackRange;           // 공격 범위 안에 있음

		[Serializable]
		public struct AttackTargetSelector
		{
			public AnimationCurve AttackRangeCurve;
			public float selectPoint;
			public UnitInteractiveInfo selectTarget;
		}
		private AttackTargetSelector attackTargetSelector;
		private UnitInteractiveInfo AttackTarget { get; set; } // 공격 타겟
		public override void OnUpdateInit()
		{
			ThisUnitData = ThisContainer.GetData<FireunitData>();

			InitFlag();
		}
		void InitFlag()
		{
			IsInVisualRange = false;
			IsInActionRange = false;
			IsInAttackRange = false;
			TempIsInVisualRange = false;
			TempIsInActionRange = false;
			TempIsInAttackRange = false;
		}
		void TempFlagUpdate()
		{
			TempIsInVisualRange = false;
			TempIsInActionRange = false;
			TempIsInAttackRange = false;
		}
		void ApplyFlag()
		{
			IsInVisualRange = TempIsInVisualRange;
			IsInActionRange = TempIsInActionRange;
			IsInAttackRange = TempIsInAttackRange;
		}
		public override void IsBeforeValueUpdate()
		{
			ThisUnitPosition =  ThisTransform.position;
			ThisUnitPosition =  ThisTransform.forward;
			ThisUnitLookUP =  ThisTransform.up;

			TempFlagUpdate();

			attackTargetSelector.selectPoint = 0;
			attackTargetSelector.selectTarget = null;
		}
		public override void IsAfterValueUpdate()
		{
			ApplyFlag();
			if(ThisContainer.TryGetComponent<IFireunitStateControl>(out var control))
			{

			}
		}
		public override void OnComputeTarget(MemberInteractiveInfo memberInteractiveInfo)
		{
			if(memberInteractiveInfo is UnitInteractiveInfo toTargetInfo)
			{
				if(toTargetInfo.DiplomacyType == FactionDiplomacyType.Enemy_Faction)
				{
					if(toTargetInfo.IsInVisualRange)
					{
						TempIsInVisualRange = true;
					}
					if(toTargetInfo.IsInActionRange)
					{
						TempIsInActionRange = true;
					}
					if(toTargetInfo.IsInAttackRange)
					{
						TempIsInAttackRange = true;
					}
				}
			}
		}
	}
}
