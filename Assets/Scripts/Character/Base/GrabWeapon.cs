namespace BC.Character
{
	using RootMotion.FinalIK;

	using UnityEngine;

	public class GrabWeapon : MonoBehaviour
	{
		[Header("From Character")]
		public Transform weaponHoldPivot;
		public Transform weaponHoldAim;
		public AimIK aimIK;
		public LimbIK leftArmIK, rightArmIK;
		[Header("From Weapon")]
		public Transform weaponPivot;
		public Transform weaponPivotAim;
		public Transform leftHand, rightHand;

		[Header("Offset")]
		public Vector3 leftHandPositionOffset;
		public Vector3 leftHandRotationOffset;

		public Vector3 rightHandPositionOffset;
		public Vector3 rightHandRotationOffset;

		[Header("Enable IK")]
		public bool updateWeaponPivot;
		public bool updateAim;
		public bool updateLeftIK;
		public bool updateRightIK;
		private void Awake()
		{
			Init();
		}
		private void OnEnable()
		{
			Init();
		}
		private void Init()
		{
			if(updateAim && aimIK != null && aimIK.enabled) aimIK.enabled = false;
			if(updateLeftIK && leftArmIK != null && leftArmIK.enabled) leftArmIK.enabled = false;
			if(updateRightIK && rightArmIK != null && rightArmIK.enabled) rightArmIK.enabled = false;
			if(updateWeaponPivot && weaponPivot != null && weaponHoldPivot != null)
			{
				weaponPivot.SetPositionAndRotation(weaponHoldPivot.position, weaponHoldPivot.rotation);
				weaponPivot.localScale = weaponHoldPivot.localScale;

				if(weaponHoldAim != null && weaponPivotAim != null)
				{
					weaponHoldAim.SetPositionAndRotation(weaponPivotAim.position, weaponPivotAim.rotation);
					weaponHoldAim.localScale = weaponPivotAim.localScale;
				}
			}
		}
		private void LateUpdate()
		{

			if(updateWeaponPivot && weaponPivot != null && weaponHoldPivot != null)
			{
				weaponPivot.SetPositionAndRotation(weaponHoldPivot.position, weaponHoldPivot.rotation);
				weaponPivot.localScale = weaponHoldPivot.localScale;
			}

			/// Update AimIK
			if(updateAim && aimIK != null)
			{
				if(aimIK.enabled) aimIK.enabled = false;
				aimIK.solver.Update();
			}

			if(updateWeaponPivot && weaponPivot != null && weaponHoldPivot != null)
			{
				weaponPivot.SetPositionAndRotation(weaponHoldPivot.position, weaponHoldPivot.rotation);
				weaponPivot.localScale = weaponHoldPivot.localScale;
			}

			/// Update Left arm IK (we don't want another FBBIK pass, LimbIK is much faster)
			if(updateRightIK && rightArmIK != null && rightHand != null)
			{
				rightArmIK.solver.IKPosition = rightHand.position + rightHandPositionOffset;
				rightArmIK.solver.IKRotation = rightHand.rotation * Quaternion.Euler(rightHandRotationOffset);

				rightArmIK.solver.Update();
			}
			if(updateLeftIK && leftArmIK != null && leftHand != null)
			{
				leftArmIK.solver.IKPosition = leftHand.position + leftHandPositionOffset;
				leftArmIK.solver.IKRotation = leftHand.rotation * Quaternion.Euler(leftHandRotationOffset);

				leftArmIK.solver.Update();
			}
		}
	}

}