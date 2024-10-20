using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

namespace BC.Character
{
	public class CharacterAgent : ComponentBehaviour, ICharacterAgent, IAnimatorStateChangeListener
	{
		[ShowInInspector, ReadOnly]
		public IFireunitData UnitData { get; set; }
		[ShowInInspector, ReadOnly]
		public ICharacterAgent.ITransformPose TransformPose { get; set; }
		[ShowInInspector, ReadOnly]
		public ICharacterAgent.IAnimation Animation { get; set; }
		[ShowInInspector, ReadOnly]
		public ICharacterAgent.IWeapon Weapon { get; set; }

		[ShowInInspector, ReadOnly]
		public AnimatorComponent.State CurrentAnimatorState { get; set; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			Init();
		}
		[Button]
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

		void IAnimatorStateChangeListener.OnAnimatorStateEnter(AnimatorComponent.State state)
		{

		}

		void IAnimatorStateChangeListener.OnAnimatorStateExit(AnimatorComponent.State state)
		{

		}
	}
}
