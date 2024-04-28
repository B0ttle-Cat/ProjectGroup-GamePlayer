using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.Character
{

	public class CharacterAnimator : AnimatorComponent, ICharacterAgent.MoveSpeed, ICharacterAgent.IsAimTarget
	{
		[SerializeField, InlineProperty, HideLabel, Title("Animator_Parameters")]
		private SD_Character_Animator_Parameters parameters = new SD_Character_Animator_Parameters();
#if UNITY_EDITOR
		public override void BaseValidate()
		{
			base.BaseValidate();
			parameters.animator = ThisAnimator;
		}
#endif
		public override void BaseAwake()
		{
			base.BaseAwake();
			parameters.animator = ThisAnimator;
		}

		public void OnAimTarget(bool aimTarget)
		{
			if(parameters == null) return;
			parameters.Aimming = aimTarget;
		}

		public void OnUpdateMoveSpeed(float moveSpeed)
		{
			if(parameters == null) return;
			parameters.Move_Speed = moveSpeed;
		}
	}
}
