using System.Collections.Generic;

using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

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

		public List<IStartSetup> startSetupList;

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
			startSetupList = new List<IStartSetup>();

			if(startLevelData.MapSetting != null && ThisObject is IGetLowLevelAIManager manager)
			{
				if(manager.LowLevelAI.ThisContainer.TryGetComponent<CreateMapObject>(out var map))
				{
					if(map is IStartSetup setter)
					{
						map.MapSetting = startLevelData.MapSetting;
						StartSetupListAdd(setter);
					}
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<CreateFactionObject>(out var faction))
			{
				if(faction is IStartSetup setter)
				{
					faction.FactionSetting = startLevelData.FactionSetting;
					StartSetupListAdd(setter);
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<UnitInteractiveComputer>(out var unitInteractiveComputer))
			{
				SetupDiplomacyData(unitInteractiveComputer.ThisContainer);
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<CreateCharacterObject>(out var character))
			{
				if(character is IStartSetup setter)
				{
					character.UnitSetting = startLevelData.UnitSetting;
					character.SpawnList = startLevelData.SpawnList;
					StartSetupListAdd(setter);
				}
			}

			void StartSetupListAdd(IStartSetup setter)
			{
				setter.IsCompleteSetting = false;
				startSetupList.Add(setter);
				setter.OnStartSetting();
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

			int length = startSetupList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				if(startSetupList[i].IsCompleteSetting) continue;
				return;
			}

			IsCompleteSetting = true;
		}


		public void SetupDiplomacyData(ContainerObject setupContainer)
		{
			if(!setupContainer.TryGetData<DiplomacyData>(out var diplomacyData))
			{
				diplomacyData = setupContainer.AddData<DiplomacyData>();
			}

			diplomacyData.diplomacyTypeList = new Dictionary<(int, int), FactionDiplomacyType>();
			var diplomacyInfoList = startLevelData.FactionSetting.diplomacyInfoList;

			int length = diplomacyInfoList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var diplomacyInfo = diplomacyInfoList[i];
				diplomacyData.diplomacyTypeList.Add((diplomacyInfo.FactionActor, diplomacyInfo.FactionTarget), diplomacyInfo.FactionDiplomacy);
			}
		}
	}
}
