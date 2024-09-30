using BC.Base;
using BC.OdccBase;

using UnityEngine;

namespace BC.Character
{
	public class CharacterModel : ModelComponent, ICharacterAgent.ITransformPose
	{
		[SerializeField]
		private Transform hitPivot;

		public Pose WorldPose => ThisTransform.GetWorldPose();
		public Pose LocalPose => ThisTransform.GetLocalPose();
		public Vector3 HitPosition { get => (hitPivot == null ? (WorldPose.position + (Vector3.up * 0.5f)) : hitPivot.position); }


		void ICharacterAgent.ITransformPose.OnUpdatePose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.SetPositionAndRotation(position, rotation);
		}
		void ICharacterAgent.ITransformPose.OnUpdateLocalPose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.SetLocalPositionAndRotation(position, rotation);
		}
	}
}
