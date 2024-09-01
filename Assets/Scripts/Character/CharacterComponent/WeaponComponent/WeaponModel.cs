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

		[Header("Cartridge")]
		public GameObject bulletPrefab;
		public GameObject casePrefab;

		[Header("IK & Pivot")]
		public Transform WeaponPivot;
		public Transform leftHandPivot;
		public Transform rightHandPivot;
		public Transform muzzlePivot;
		public Transform ejectionPort;

#if UNITY_EDITOR
		public override void BaseReset() { }
		public override void BaseValidate() { }
#endif

		public override void BaseAwake()
		{
			if(bulletPrefab == null)
				bulletPrefab = null;
			if(casePrefab == null)
				casePrefab = null;

			bulletPrefab?.SetActive(false);
			casePrefab?.SetActive(false);
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
		public override void BaseUpdate()
		{

		}
	}
}
