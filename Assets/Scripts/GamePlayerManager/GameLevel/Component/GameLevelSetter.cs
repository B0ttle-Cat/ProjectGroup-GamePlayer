using System.Collections.Generic;

using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class GameLevelSetter : ComponentBehaviour, IStartSetup
	{
		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				if(startSetupList == null) return false;
				int length = startSetupList.Count;
				for(int i = 0 ; i < length ; i++)
				{
					if(!startSetupList[i].IsCompleteSetting)
						return false;
				}
				isCompleteSetting = true;
				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}
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
			startSetupList = new List<IStartSetup>();

			if(startLevelData.MapSetting != null && ThisObject is IGetLowLevelAIManager manager)
			{
				if(manager.ThisContainer.TryGetComponent<CreateMapObject>(out var map))
				{
					map.MapSetting = startLevelData.MapSetting;
					if(map is IStartSetup setter)
					{
						StartSetupListAdd(setter);
					}
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponentInChild<CreateFactionObject>(out var faction))
			{
				faction.FactionSetting = startLevelData.FactionSetting;
				if(faction is IStartSetup setter)
				{
					StartSetupListAdd(setter);
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponentInChild<UnitInteractiveComputer>(out var unitInteractiveComputer))
			{
				SetupDiplomacyData(unitInteractiveComputer.ThisContainer);
			}


			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<CreateCharacterObject>(out var character))
			{
				character.GamePlaySetting = startLevelData.GamePlaySetting;
				if(character is IStartSetup setter)
				{
					StartSetupListAdd(setter);
				}
			}

			void StartSetupListAdd(IStartSetup setter)
			{
				startSetupList.Add(setter);
				setter.ThisMono.enabled = true;
			}
		}
		public override void BaseUpdate()
		{
			OnUpdateSetting();
		}
		public void OnUpdateSetting()
		{
			if(IsCompleteSetting)
			{
				enabled = false;
			}
		}

		public void SetupDiplomacyData(ContainerObject setupContainer)
		{
			if(!setupContainer.TryGetData<DiplomacyData>(out var diplomacyData))
			{
				diplomacyData = setupContainer.AddData<DiplomacyData>();
			}

			diplomacyData.diplomacyTypeList = new Dictionary<(int, int), FactionDiplomacyType>();
			var factionInfoList = startLevelData.FactionSetting.factionSettingList;

			int length = factionInfoList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var factionInfo = factionInfoList[i];
				int length2 = factionInfo.DiplomacyInfoList.Count;
				for(int ii = 0 ; ii < length2 ; ii++)
				{
					var diplomacyInfo = factionInfo.DiplomacyInfoList[ii];
					diplomacyData.diplomacyTypeList.Add((factionInfo.FactionIndex, diplomacyInfo.FactionTarget), diplomacyInfo.FactionDiplomacy);
				}
			}
		}
	}
}
