using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using static BC.GamePlayerManager.StartGameSetting;

namespace BC.GamePlayerManager
{
	public class CreateCharacterObject : ComponentBehaviour, IStartSetup
	{
		[SerializeField]
		private CharacterObject characterPrefab;

		private Queue<(StartUnitSettingCharacter, SpawnData)> characterSettingDatas;

		private IGetLowLevelAIManager lowLevelAIManager;

		[ShowInInspector, ReadOnly]
		public bool IsCompleteSetting { get; private set; }
		public StartUnitSetting UnitSetting { get; internal set; }
		public List<SpawnAnchor> SpawnList { get; internal set; }


		protected override void Disposing()
		{
			base.Disposing();

			characterPrefab = null;
			characterSettingDatas = null;
			UnitSetting = null;
			lowLevelAIManager = null;
		}
		public override void BaseAwake()
		{
			base.BaseAwake();

			if(characterPrefab != null)
				characterPrefab.gameObject.SetActive(false);

			ThisContainer.TryGetObject<IGetLowLevelAIManager>(out lowLevelAIManager);
		}


		public override async void BaseEnable()
		{
			IsCompleteSetting = false;

			if(characterPrefab == null)
			{
				OnStopSetting();
				return;
			}
			if(UnitSetting == null || SpawnList == null || UnitSetting.characterDatas == null)
			{
				OnStopSetting();
				return;
			}

			var computer = await lowLevelAIManager.ThisContainer.AwaitGetComponentInChild<MapPathPointComputer>(item=>item.IsCompleteUpdate);

			characterSettingDatas = new Queue<(StartUnitSettingCharacter, SpawnData)>();

			var list = UnitSetting.characterDatas;
			var gorup = list.GroupBy(item => (item.FactionIndex, item.TeamIndex));
			int length = list.Count;

			for(int i = 0 ; i < length ; i++)
			{
				var unit = list[i];

				var findGroupCount = gorup.FirstOrDefault(item=>{
					var _item = item.First();
					return _item.FactionIndex == unit.FactionIndex && _item.TeamIndex == unit.TeamIndex;
				}).Count();

				SpawnData spawn = null;
				int findAnchorIndex = SpawnList.FindIndex(_item => _item.FactionIndex == unit.FactionIndex && _item.TeamIndex == unit.TeamIndex);
				if(findAnchorIndex>=0)
				{
					if(computer.TrySelectAnchorIndex(SpawnList[findAnchorIndex].AnchorIndex, out var spawnAnchor))
					{
						spawn = new SpawnData() {
							targetAnchor = spawnAnchor,
							totalUnitCount = findGroupCount,
							unitIndex = unit.UnitIndex,
							targetRadius = 2f,
						};
					}
				}

				characterSettingDatas.Enqueue((unit, spawn));
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

			if(characterSettingDatas == null) return;

			int length = characterSettingDatas.Count;
			if(length > 0)
			{
				var item = characterSettingDatas.Dequeue();
				CharacterObject characterObject = InstantiateCharacterObject(item.Item1, item.Item2);
				characterObject.gameObject.SetActive(true);
				return;
			}

			IsCompleteSetting = true;
		}



		private CharacterObject InstantiateCharacterObject(StartUnitSettingCharacter characterSettingData, SpawnData spawnData)
		{
			characterPrefab.gameObject.SetActive(false);
			CharacterObject characterObject = Instantiate(characterPrefab);
			characterObject.transform.ResetLcoalPose(ThisTransform);

			if(!characterObject.ThisContainer.TryGetData<CharacterData>(out var characterData))
			{
				characterData = characterObject.ThisContainer.AddData<CharacterData>();
			}

			characterData.FactionIndex = characterSettingData.FactionIndex;
			characterData.TeamIndex = characterSettingData.TeamIndex;
			characterData.UnitIndex = characterSettingData.UnitIndex;
			characterData.CharacterResourcesKey = characterSettingData.CharacterResourcesKey;
			characterData.WeaponResourcesKey = characterSettingData.WeaponResourcesKey;
			characterObject.UpdateObjectName();

			characterObject.ThisContainer.RemoveData<SpawnData>();
			characterObject.ThisContainer.AddData<SpawnData>(spawnData);

			return characterObject;
		}
	}
}
