using BC.ODCC;

using RootMotion.FinalIK;

namespace BC.Character
{
	public class CharacterModel : ComponentBehaviour
	{
#if UNITY_EDITOR
		public override void BaseReset() { }
		public override void BaseValidate() { }
#endif
		public AimController aimController;
		public GrabWeapon grabWeapon;

		public override void BaseAwake()
		{
			aimController = GetComponent<AimController>();
			grabWeapon = GetComponent<GrabWeapon>();

			aimController.enabled = false;
			grabWeapon.enabled = false;
		}
		public override void BaseDestroy()
		{

		}
		public override void BaseEnable()
		{
			if(ThisObject.ThisContainer.TryGetComponent<WeaponModel>(out var Weapon))
			{
				grabWeapon.weaponPivot = Weapon.WeaponPivot;
				grabWeapon.weaponPivotAim = Weapon.muzzlePivot;
				grabWeapon.leftHand = Weapon.leftHandPivot;
				grabWeapon.rightHand = Weapon.rightHandPivot;
			}
		}
		public override void BaseDisable()
		{

		}
		public override void BaseStart()
		{

		}
		public override void BaseUpdate()
		{

		}
	}
}
