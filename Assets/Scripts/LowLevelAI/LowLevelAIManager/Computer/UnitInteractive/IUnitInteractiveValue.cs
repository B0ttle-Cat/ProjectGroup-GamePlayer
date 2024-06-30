using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="UnitInteractiveInfo"/>
	/// </summary>
	public interface IUnitInteractiveValue : IOdccComponent
	{
		public IFireunitData ThisUnitData { get; set; }

		public float ThisUnitRadius { get; set; }
		public Vector3 ThisUnitPosition { get; set; }
		public Vector3 ThisUnitLookAt { get; set; }
		public Vector3 ThisUnitLookUP { get; set; }


		public float ThisVisualRange { get; set; }
		public float ThisActionRange { get; set; }
		public float ThisAttackRange { get; set; }

		void OnUpdateInit();
		void OnUpdateValue();
	}
}
