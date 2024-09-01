using BC.OdccBase;

using RootMotion.FinalIK;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.Character
{
	public class CharacterAnimator : AnimatorComponent, ICharacterAgent.IAnimation
	{
		[SerializeField, InlineProperty, HideLabel, Title("Animator_Parameters")]
		private SD_Character_Animator_Parameters parameters = new SD_Character_Animator_Parameters();

		public AimController aimController;
		public GrabWeapon grabWeapon;

#if UNITY_EDITOR
		public override void BaseValidate()
		{
			base.BaseValidate();
			parameters.ThisAnimator = ThisAnimator;

			aimController = GetComponent<AimController>();
			grabWeapon = GetComponent<GrabWeapon>();

			aimController.enabled = false;
			grabWeapon.enabled = false;
		}
#endif
		public override void BaseReset()
		{

		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			parameters.ThisAnimator = ThisAnimator;
		}
		public override void BaseEnable()
		{
			if(ThisContainer.TryGetComponent<WeaponModel>(out var weaponModel))
			{
				grabWeapon.weaponPivot = weaponModel.WeaponPivot;
				grabWeapon.weaponPivotAim = weaponModel.muzzlePivot;
				grabWeapon.leftHand = weaponModel.leftHandPivot;
				grabWeapon.rightHand = weaponModel.rightHandPivot;
			}
		}
		public void OnAimTarget(bool aimTarget, Vector3 aimPos)
		{
			if(parameters == null) return;
			parameters.Aiming = aimTarget;
			aimController.enabled = aimTarget;
			if(aimTarget)
			{
				aimController.target.position = aimPos;
			}
		}

		public void OnUpdateMoveSpeed(float moveSpeed)
		{
			if(parameters == null) return;
			parameters.Move_Speed = moveSpeed;
		}


		public void OnCallChangeFace()
		{

		}
	}
}
