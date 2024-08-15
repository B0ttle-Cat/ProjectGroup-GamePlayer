using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IUnitPoseValue : IPlayValue
	{
		public Vector3 ThisUnitPosition { get; set; }
		public float ThisUnitRadius { get; set; }
		public Vector3 ThisUnitLookAt { get; set; }
		public Vector3 ThisUnitLookUP { get; set; }
	}


	public class FireunitPoseValue : DataObject, IUnitPoseValue
	{
		private IFireunitData unitData;
		private float thisUnitRadius;
		private Vector3 thisUnitPosition;
		private Vector3 thisUnitLookAt;
		private Vector3 thisUnitLookUP;
		public IFireunitData UnitData { get => unitData; set => unitData=value; }

		public float ThisUnitRadius { get => thisUnitRadius; set => thisUnitRadius=value; }
		public Vector3 ThisUnitPosition { get => thisUnitPosition; set => thisUnitPosition=value; }
		public Vector3 ThisUnitLookAt { get => thisUnitLookAt; set => thisUnitLookAt=value; }
		public Vector3 ThisUnitLookUP { get => thisUnitLookUP; set => thisUnitLookUP=value; }

	}
}
