using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.Character
{
	public class CharacterAgent : ComponentBehaviour, ICharacterAgent, ICharacterAgent.ITransformPose, IAnimatorStateChangeListener
	{
		[SerializeField]
		private Transform hitPivot;

		public IFireunitData UnitData { get; set; }
		public ICharacterAgent.ITransformPose TransformPose { get; set; }
		public ICharacterAgent.IAnimation Animation { get; set; }
		public ICharacterAgent.IWeapon Weapon { get; set; }

		public Vector3 HitPosition { get => (hitPivot == null ? (TransformPose.WorldPose.position + (Vector3.up * 0.5f)) : hitPivot.position); }

		public AnimatorComponent.State CurrentAnimatorState { get; set; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			Init();
		}
		internal void Init()
		{
			UnitData ??= ThisContainer.GetData<IFireunitData>();
			TransformPose ??= ThisContainer.GetComponent<ICharacterAgent.ITransformPose>();
			Animation ??= ThisContainer.GetComponent<ICharacterAgent.IAnimation>();
			Weapon ??= ThisContainer.GetComponent<ICharacterAgent.IWeapon>();
		}

		public override void BaseEnable()
		{
			base.BaseEnable();
		}

		void ICharacterAgent.ITransformPose.OnUpdatePose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.SetPositionAndRotation(position, rotation);
		}
		void ICharacterAgent.ITransformPose.OnUpdateLocalPose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.SetLocalPositionAndRotation(position, rotation);
		}

		void IAnimatorStateChangeListener.OnAnimatorStateEnter(AnimatorComponent.State state)
		{

		}

		void IAnimatorStateChangeListener.OnAnimatorStateExit(AnimatorComponent.State state)
		{

		}
	}
}
