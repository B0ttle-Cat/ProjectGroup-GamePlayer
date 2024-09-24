using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IUnitPoseValue : IOdccData
	{
		public Vector3 ThisUnitPosition { get; set; }
		public float ThisUnitRadius { get; set; }
		public Vector3 ThisUnitLookAt { get; set; }
		public Vector3 ThisUnitLookUP { get; set; }

		public Vector3 ThisDamageUIPosition { get; set; }
	}


	public class FireunitPoseValue : DataObject, IUnitPoseValue
	{
		private float thisUnitRadius;
		private Vector3 thisUnitPosition;
		private Vector3 thisUnitLookAt;
		private Vector3 thisUnitLookUP;
		private Vector3 thisDamageUIPosition;

		public float ThisUnitRadius { get => thisUnitRadius; set => thisUnitRadius=value; }
		public Vector3 ThisUnitPosition { get => thisUnitPosition; set => thisUnitPosition=value; }
		public Vector3 ThisUnitLookAt { get => thisUnitLookAt; set => thisUnitLookAt=value; }
		public Vector3 ThisUnitLookUP { get => thisUnitLookUP; set => thisUnitLookUP=value; }
		public Vector3 ThisDamageUIPosition { get => thisDamageUIPosition; set => thisDamageUIPosition=value; }

		protected override void Disposing()
		{
		}
	}
}
