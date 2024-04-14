using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.VFX;

namespace BC.Character
{

	[RequireComponent(typeof(Animator))]
	public class WeaponAnimation : ComponentBehaviour, IWeaponFire, IWeaponReload
	{
		[SerializeField]
		private WeaponModel model;

		[Header("Animation")]
		public Animator animator;

		[Header("Sound")]
		public AudioSource audioSource;
		public AudioClip fireSoundClip;

		[Header("Effect")]
		public VisualEffect muzzleFlame;

		[Header("Cartridge")]
		public GameObject bulletPrefab;
		public GameObject casePrefab;

		public override void BaseAwake()
		{
			if(bulletPrefab == null)
				bulletPrefab = null;
			if(casePrefab == null)
				casePrefab = null;

			bulletPrefab?.SetActive(false);
			casePrefab?.SetActive(false);
		}

		public override void BaseEnable()
		{
			model = ThisObject is WeaponModel _model ? _model : null;


			if(animator == null) animator = null;
			if(audioSource == null) audioSource = null;
			if(fireSoundClip == null) fireSoundClip = null;
			if(muzzleFlame == null) muzzleFlame = null;
			if(bulletPrefab == null) bulletPrefab = null;
			if(casePrefab == null) casePrefab = null;
		}

		[Header("BulletRandom")]
		public float randomBulletDiraction = 0.01f;
		public float bulletSpeed = 3f;

		[Header("CaseRandom")]
		public Vector2 randomForce = new Vector2(1f, 2f);
		public Vector2 randomTorque = new Vector2(-20f, -30f);

		public float randomCaseDiraction = 0.25f;
		public Vector2 randomDrag = new Vector2(1f, 2f);
		public Vector2 randomAngularDrag = new Vector2(0.05f, 0.1f);

		[Button]
		void IWeaponFire.DoFire()
		{
			if(model is null) return;

			animator?.SetTrigger("DoFire");

			if(audioSource is not null && fireSoundClip is not null)
			{
				audioSource.PlayOneShot(fireSoundClip);
			}

			if(muzzleFlame is not null)
			{
				VisualEffect newEffect = GameObject.Instantiate(muzzleFlame);
				newEffect.transform.position = model.muzzlePivot.position;
				newEffect.transform.rotation = model.muzzlePivot.rotation;
				newEffect.transform.localScale = model.muzzlePivot.localScale;
				newEffect.Play();
				Destroy(newEffect.gameObject, 0.25f);
			}

			if(casePrefab is not null)
			{
				GameObject newCase = GameObject.Instantiate(casePrefab);
				newCase.transform.position = model.ejectionPort.position;
				newCase.transform.rotation = model.ejectionPort.rotation;
				newCase.transform.localScale = model.ejectionPort.localScale;
				newCase.SetActive(true);
				var newCaseRigidbody = newCase.GetComponent<Rigidbody>();

				newCaseRigidbody.linearDamping = Random.Range(randomDrag.x, randomDrag.y);
				newCaseRigidbody.angularDamping = Random.Range(randomAngularDrag.x, randomAngularDrag.y);

				Vector3 force = (newCaseRigidbody.transform.up * 0.1f + newCaseRigidbody.transform.right * 0.9f).normalized;
				force += Random.insideUnitSphere * randomCaseDiraction;
				force = force.normalized;
				newCaseRigidbody.AddForce(force * Random.Range(randomForce.x, randomForce.y), ForceMode.Impulse);
				newCaseRigidbody.AddTorque(Vector3.Cross(force.normalized, newCaseRigidbody.transform.forward) * Random.Range(randomTorque.x, randomTorque.y), ForceMode.Impulse);
				newCaseRigidbody.maxAngularVelocity = randomTorque.y;
				Destroy(newCase, 3f);
			}
			if(bulletPrefab is not null)
			{
				GameObject newBullet = GameObject.Instantiate(bulletPrefab);
				newBullet.transform.position = model.muzzlePivot.position;
				newBullet.transform.rotation = model.muzzlePivot.rotation;
				newBullet.transform.localScale = model.muzzlePivot.localScale;

				if(EventManager.Call<IBulletProjectile, bool>(false,
					item => item.CreateBullet(ThisObject as IShotterProjectile, newBullet)))
				{
					/// IBulletProjectile �� ���ؼ� �߻�ü�� ������.
				}
				else
				{
					/// IBulletProjectile �� ���ؼ� �߻�ü�� �������� ����.
					newBullet.SetActive(true);

					Vector3 force = newBullet.transform.forward;
					force += Random.insideUnitSphere * randomBulletDiraction;
					force = force.normalized;

					newBullet.transform.LookAt(newBullet.transform.position + force, Vector3.up);
					newBullet.GetComponent<Rigidbody>()?.AddForce(force * bulletSpeed, ForceMode.Impulse);
					Destroy(newBullet, 3f);
				}
			}
		}

		void IWeaponReload.DoReload()
		{

		}
	}
}
