using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitInteractiveValue : MemberInteractiveValue, IUnitInteractiveValue
	{
		public IFireunitData ThisUnitData { get; set; }
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
		public override void OnUpdateInit()
		{
			ThisUnitData = ThisContainer.GetData<FireunitData>();
		}
		public override void IsBeforeValueUpdate()
		{

		}
		public override void OnValueRefresh()
		{
			ThisUnitPosition =  ThisTransform.position;
			ThisUnitPosition =  ThisTransform.forward;
			ThisUnitLookUP =  ThisTransform.up;
		}
		public override void IsAfterValueUpdate()
		{

		}
	}
}
