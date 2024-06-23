using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IUnitTarget : IOdccComponent
	{
		public float TargetingRadius { get; set; }
		public Vector3 TargetingPosition { get; set; }
	}
}
