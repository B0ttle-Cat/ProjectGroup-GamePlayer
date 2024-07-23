using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="UnitInteractiveInfo"/>
	/// </summary>
	public interface IUnitInteractiveValue : IMemberInteractiveValue
	{
		// Meta
		public IFireunitData ThisUnitData { get; set; }

		// Value
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

		public float ClampVisualRange(float deltaRange)
		{
			return Mathf.Clamp(deltaRange, MinVisualRange, MaxVisualRange);
		}
		public float ClampAttackRange(float deltaRange)
		{
			return Mathf.Clamp(deltaRange, MinAttackRange, MaxAttackRange);
		}
	}
}
