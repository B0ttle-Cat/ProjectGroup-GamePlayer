using BC.Base;
using BC.ODCC;

namespace BC.Character
{
	public class CharacterObject : ObjectBehaviour, IShotterProjectile, ITargetProjectile
	{
#if UNITY_EDITOR
		public override void BaseReset() { }
		public override void BaseValidate()
		{
			if(!ThisContainer.TryGetData<CharacterData>(out _))
			{
				ThisContainer.AddData<CharacterData>();
			}
			if(ThisContainer.TryGetComponent<CharacterResourcesSetup>(out var characterResourcesSetup))
			{
				characterResourcesSetup.enabled = false;
			}
		}
#endif
		private CharacterData characterData;

		public CharacterModel Model { get; private set; }
		public WeaponModel Weapon { get; private set; }


		public override void BaseAwake()
		{
			if(!ThisContainer.TryGetData<CharacterData>(out characterData))
			{
				characterData = ThisContainer.AddData<CharacterData>();
			}

			if(characterData != null && ThisContainer.TryGetComponent<CharacterResourcesSetup>(out var resourcesSetup))
			{
				resourcesSetup.enabled = false;
				resourcesSetup.ResourcesSetup(characterData.CharacterKey, characterData.WeaponeKey, (m, w) =>
				{
					Model = m;
					Weapon = w;
					resourcesSetup.enabled = false;
					UpdateObjectName();
				});
				resourcesSetup.enabled = true;
			}
		}
		public void UpdateObjectName()
		{
			if(ThisContainer.TryGetData<CharacterData>(out characterData))
			{
				gameObject.name = $"{characterData.FactionIndex:00} | {characterData.TeamIndex:00} | {characterData.UnitIndex:00} Character " +
					$"({(Model == null ? "" : Model.name)} | {(Weapon == null ? "" : Weapon.name)})";
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