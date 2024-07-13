using BC.LowLevelAI;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class GameLevelSetter : ComponentBehaviour, IStartSetup
	{
#if UNITY_EDITOR
		public override void BaseValidate()
		{
			base.BaseValidate();

			if(!ThisContainer.TryGetData<StartLevelData>(out _))
			{
				ThisContainer.AddData<StartLevelData>();
			}
		}
#endif
		private StartLevelData startLevelData;

		private IStartSetup mapCreator;
		private IStartSetup factionCreator;
		private IStartSetup characterCreator;

		[SerializeField, ReadOnly]
		private bool isCompleteSetting = false;
		public bool IsCompleteSetting { get => isCompleteSetting; set => isCompleteSetting=value; }


		public override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<StartLevelData>(out startLevelData))
			{
				startLevelData = ThisContainer.AddData<StartLevelData>();
			}
		}

		public override void BaseEnable()
		{
			IsCompleteSetting = false;
			mapCreator = null;
			characterCreator = null;

			if(startLevelData.MapSetting != null && ThisObject is IGetLowLevelAIManager manager)
			{
				if(manager.LowLevelAI.ThisContainer.TryGetComponent<CreateMapObject>(out var map))
				{
					if(map is IStartSetup setter)
					{
						map.MapSetting = startLevelData.MapSetting;

						mapCreator = setter;
						setter.OnStartSetting();
					}
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<CreateFactionObject>(out var faction))
			{
				if(faction is IStartSetup setter)
				{
					faction.FactionSetting = startLevelData.FactionSetting;
					factionCreator = setter;
					setter.OnStartSetting();
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<CreateCharacterObject>(out var character))
			{
				if(character is IStartSetup setter)
				{
					character.UnitSetting = startLevelData.UnitSetting;
					character.SpawnList = startLevelData.SpawnList;
					characterCreator = setter;
					setter.OnStartSetting();
				}
			}
		}
		public override void BaseUpdate()
		{
			OnUpdateSetting();
		}
		public void OnStartSetting()
		{
			enabled = true;
		}

		public void OnStopSetting()
		{
			enabled = false;
		}

		public void OnUpdateSetting()
		{
			if(IsCompleteSetting)
			{
				OnStopSetting();
				return;
			}

			if(mapCreator !=  null && !mapCreator.IsCompleteSetting)
			{
				return;
			}
			if(characterCreator !=  null && !characterCreator.IsCompleteSetting)
			{
				return;
			}

			IsCompleteSetting = true;
		}
	}
}
