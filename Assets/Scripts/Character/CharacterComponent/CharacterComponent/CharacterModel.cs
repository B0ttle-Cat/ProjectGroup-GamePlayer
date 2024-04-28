using RootMotion.FinalIK;

namespace BC.Character
{
	public class CharacterModel : ModelObject
	{
		public override bool IsReady { get; protected set; } = false;
#if UNITY_EDITOR
		public override void BaseReset()
		{
			aimController = GetComponent<AimController>();
			grabWeapon = GetComponent<GrabWeapon>();

			aimController.enabled = false;
			grabWeapon.enabled = false;
		}
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
			if(ThisContainer.ParentContainer.TryGetChildObject<WeaponModel>(out var Weapon))
			{
				grabWeapon.weaponPivot = Weapon.WeaponPivot;
				grabWeapon.weaponPivotAim = Weapon.muzzlePivot;
				grabWeapon.leftHand = Weapon.leftHandPivot;
				grabWeapon.rightHand = Weapon.rightHandPivot;
			}
			IsReady = true;
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
