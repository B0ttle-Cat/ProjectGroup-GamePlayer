using BC.Base;
using BC.ODCC;

namespace BC.Character
{
	public class CharacterObject : ObjectBehaviour, IShotterProjectile, ITargetProjectile
	{
		public ObjectBehaviour ThisObject { get => this; }
#if UNITY_EDITOR
		public override void BaseReset() { }
		public override void BaseValidate() { }
#endif
		private CharacterResourcesSetup resourcesSetup;

		public CharacterModel Model { get; private set; }
		public WeaponModel Weapon { get; private set; }

		public override void BaseAwake()
		{
			if(ThisContainer.TryGetComponent<CharacterResourcesSetup>(out resourcesSetup))
			{
				resourcesSetup.enabled = true;
				resourcesSetup.ResourcesSetup((m, w) =>
				{
					Model = m;
					Weapon = w;

					Model.gameObject.SetActive(true);
					Weapon.gameObject.SetActive(true);
					resourcesSetup.enabled = false;
				});
			}
			else
			{

			}
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