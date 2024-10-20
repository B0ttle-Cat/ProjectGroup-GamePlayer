using System.Collections.Generic;

using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

namespace BC.GamePlayManager
{
	public class GameLevelSetter : ComponentBehaviour, IStartSetup, IOdccUpdate
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
		private StartLevelData startLevelData;

		public List<IStartSetup> startSetupList;

		public override void BaseAwake()
		{
			base.BaseAwake();
			ThisContainer.TryGetData<StartLevelData>(out startLevelData);
		}

		public override void BaseEnable()
		{
			startSetupList = new List<IStartSetup>();

			if(startLevelData.MapSetting != null && ThisObject is IGamePlayManager manager)
			{
				var map = manager.MapCreatorComponent;
				if(map != null)
				{
					map.MapSetting = startLevelData.MapSetting;
					if(map is IStartSetup setter)
					{
						StartSetupListAdd(setter);
					}
				}
			}

			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponentInChild<FactionCreatorComponent>(out var faction))
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


			if(startLevelData.UnitSetting != null && ThisContainer.TryGetComponent<CharacterCreatorComponent>(out var character))
			{
				character.GamePlaySetting = startLevelData.GamePlaySetting;
				character.TeamSettingList = startLevelData.TeamSetting.teamSettingList;
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
		public void BaseUpdate()
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
