using BC.ODCC;
using BC.OdccBase;

namespace BC.Character
{
	public class CharacterAgent : ComponentBehaviour, ICharacterAgent, IAnimatorStateChangeListener
	{
		public IFireunitData UnitData { get; set; }
		public ICharacterAgent.ITransformPose TransformPose { get; set; }
		public ICharacterAgent.IAnimation Animation { get; set; }
		public ICharacterAgent.IWeapon Weapon { get; set; }


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

		void IAnimatorStateChangeListener.OnAnimatorStateEnter(AnimatorComponent.State state)
		{

		}

		void IAnimatorStateChangeListener.OnAnimatorStateExit(AnimatorComponent.State state)
		{

		}
	}
}
