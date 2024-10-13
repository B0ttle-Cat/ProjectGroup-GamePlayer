using UnityEngine;
using UnityEngine.VFX;

namespace BC.Character
{
	public class WeaponModel : ModelComponent
	{
		[Header("Sound")]
		public AudioSource audioSource;
		public AudioClip fireSoundClip;

		[Header("Effect")]
		public VisualEffect muzzleFlame;

		[Header("Shoot Object")]
		public GameObject bulletPrefab;
		public GameObject cartridgePrefab;
		public Transform muzzlePivot;
		public Transform cartridgePivot;

		[Header("IK Pivot")]
		public Transform WeaponPivot;
		public Transform leftHandPivot;
		public Transform rightHandPivot;

#if UNITY_EDITOR
		public override void BaseReset() { }
		public override void BaseValidate() { }
#endif

		public override void BaseAwake()
		{
			if(bulletPrefab == null)
				bulletPrefab = null;
			if(cartridgePrefab == null)
				cartridgePrefab = null;

			bulletPrefab?.SetActive(false);
			cartridgePrefab?.SetActive(false);
		}
		public override void BaseDestroy()
		{

		}
		public override void BaseEnable()
		{
		}
		public override void BaseDisable()
		{

		}
		public override void BaseStart()
		{

		}
	}
}
