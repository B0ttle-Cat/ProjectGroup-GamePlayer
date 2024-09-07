using BC.ODCC;

namespace BC.Character
{
	public class CharacterObject : ObjectBehaviour
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
		private WeaponData weaponData;

		public CharacterData CharacterData { get => characterData; private set => characterData=value; }
		public WeaponData WeaponData { get => weaponData; private set => weaponData=value; }
		public CharacterAgent Agent { get; private set; }
		public CharacterModel Model { get; private set; }
		public WeaponModel Weapon { get; private set; }

		public override void BaseAwake()
		{
			if(!ThisContainer.TryGetData<CharacterData>(out characterData))
			{
				characterData = ThisContainer.AddData<CharacterData>();
			}
			if(!ThisContainer.TryGetData<WeaponData>(out weaponData))
			{
				weaponData = ThisContainer.AddData<WeaponData>();
			}

			if(!ThisContainer.TryGetComponent<CharacterAgent>(out var agent))
			{
				agent = ThisContainer.AddComponent<CharacterAgent>();
			}
			Agent = agent;

			if(CharacterData != null && ThisContainer.TryGetComponent<CharacterResourcesSetup>(out var resourcesSetup))
			{
				resourcesSetup.enabled = false;
				resourcesSetup.ResourcesSetup(CharacterData.CharacterResourcesKey, WeaponData.WeaponResourcesKey, (m, w) => {
					Model = m;
					Weapon = w;
					resourcesSetup.enabled = false;
					UpdateObjectName();
					Agent.Init();
				});
				resourcesSetup.enabled = true;
			}
		}
		public void UpdateObjectName()
		{
			if(CharacterData != null || ThisContainer.TryGetData<CharacterData>(out characterData))
			{
				gameObject.name = $"{CharacterData.FactionIndex:00} | {CharacterData.TeamIndex:00} | {CharacterData.UnitIndex:00} Character " +
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