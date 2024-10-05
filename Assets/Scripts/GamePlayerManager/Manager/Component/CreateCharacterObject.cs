using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.Map;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

using static BC.GamePlayerManager.TeamSetting;

namespace BC.GamePlayerManager
{
	public class CreateCharacterObject : ComponentBehaviour, IStartSetup
	{
		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				if(characterPrefab == null || UnitSetting == null || SpawnList == null) return false;
				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}

		[SerializeField]
		private CharacterObject characterPrefab;

		public UnitSetting UnitSetting { get; internal set; }
		public List<TeamSettingInfo> SpawnList { get; internal set; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			isCompleteSetting = false;
#if !UNITY_EDITOR
			if(characterPrefab != null) characterPrefab.gameObject.SetActive(false);
#endif
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			isCompleteSetting = false;
		}
		protected override void Disposing()
		{
			base.Disposing();

			characterPrefab = null;
			UnitSetting = null;
		}


		public override async void BaseEnable()
		{
			IsCompleteSetting = false;
			while(characterPrefab == null || UnitSetting == null || SpawnList == null)
			{
				await Awaitable.NextFrameAsync();
				if(!enabled) return;
			}
			var characterSettingList = new Queue<(UnitSettingInfo, SpawnData)>();
			await CreateSettingList();
			await CreateCharacterList();
			isCompleteSetting = true;

			async Awaitable CreateSettingList()
			{
				var lowLevelAIManager = await ThisContainer.AwaitGetObject<IGetLowLevelAIManager>(null, DisableCancelToken);
				var mapPathPointComputer = await lowLevelAIManager.ThisContainer.AwaitGetComponentInChild<MapPathPointComputer>(item => item.IsCompleteUpdate,DisableCancelToken);

				IsCompleteSetting = true;

				var list = UnitSetting.unitSettingList;
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
						if(mapPathPointComputer.TrySelectAnchorIndex(SpawnList[findAnchorIndex].AnchorIndex, out var spawnAnchor))
						{
							spawn = new SpawnData() {
								targetAnchor = spawnAnchor,
								totalUnitCount = findGroupCount,
								unitIndex = unit.UnitIndex,
								targetRadius = 5f,
							};
						}
					}
					characterSettingList.Enqueue((unit, spawn));
				}
			}
			async Awaitable CreateCharacterList()
			{
				if(characterSettingList == null) return;

				int count = characterSettingList.Count;
				while(characterSettingList.Count > 0)
				{
					var item = characterSettingList.Dequeue();
					Create(item.Item1, item.Item2);
				}
				async void Create(UnitSettingInfo characterData, SpawnData spawnData)
				{
					CharacterObject characterObject = await InstantiateCharacterObject(characterData, spawnData);
					count--;
				}
				while(count>0)
				{
					await Awaitable.NextFrameAsync();
				}
			}
		}

		private async Awaitable<CharacterObject> InstantiateCharacterObject(UnitSettingInfo fireunitSettingData, SpawnData spawnData)
		{
			if(characterPrefab == null) return null;

#if UNITY_EDITOR
			characterPrefab.gameObject.SetActive(false);
#endif
			var instantiateAsync = InstantiateAsync(characterPrefab);
			await instantiateAsync;
#if UNITY_EDITOR
			characterPrefab.gameObject.SetActive(true);
#endif
			CharacterObject characterObject = instantiateAsync.Result[0];
			characterObject.transform.ResetLcoalPose(ThisTransform);

			if(!characterObject.ThisContainer.TryGetData<CharacterData>(out var characterData))
			{
				characterData = characterObject.ThisContainer.AddData<CharacterData>();
			}

			characterData.FactionIndex = fireunitSettingData.FactionIndex;
			characterData.TeamIndex = fireunitSettingData.TeamIndex;
			characterData.UnitIndex = fireunitSettingData.UnitIndex;
			characterData.CharacterResourcesKey = fireunitSettingData.CharacterSetter.ResourcesSetter.CharacterResourcesKey;
			characterObject.UpdateObjectName();

			if(!characterObject.ThisContainer.TryGetData<WeaponData>(out var weaponData))
			{
				weaponData = characterObject.ThisContainer.AddData<WeaponData>();
			}
			weaponData.WeaponResourcesKey = fireunitSettingData.CharacterSetter.ResourcesSetter.WeaponResourcesKey;

			characterObject.ThisContainer.RemoveData<SpawnData>();
			characterObject.ThisContainer.AddData<SpawnData>(spawnData);

			characterObject.gameObject.SetActive(true);
			return characterObject;
		}
	}
}
