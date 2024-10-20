using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.Map;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class CharacterCreatorComponent : ComponentBehaviour, IStartSetup, ICharacterCreatorComponent
	{

		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				if(characterPrefab == null || GamePlaySetting == null || TeamSettingList == null) return false;
				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}

		[SerializeField]
		private CharacterObject characterPrefab;

		public MainPlaySetting GamePlaySetting { get; internal set; }
		public List<TeamSettingInfo> TeamSettingList { get; internal set; }

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
			GamePlaySetting = null;
		}


		public override async void BaseEnable()
		{
			IsCompleteSetting = false;
			while(characterPrefab == null || GamePlaySetting == null)
			{
				await Awaitable.NextFrameAsync();
				if(!enabled) return;
			}
			var characterSettingList = new Queue<CreateSettingInfo>();
			await CreateSettingList();
			await CreateCharacterList();
			isCompleteSetting = true;

			async Awaitable CreateSettingList()
			{
				var lowLevelAIManager = await ThisContainer.AwaitGetObject<IGamePlayManager>(null, DisableCancelToken);
				var mapPathPointComputer = await lowLevelAIManager.ThisContainer.AwaitGetComponentInChild<MapPathPointComputer>(item => item.IsCompleteUpdate,DisableCancelToken);

				IsCompleteSetting = true;

				var list = GamePlaySetting.UnitSetting.unitSettingList;
				int length = list.Count;

				for(int i = 0 ; i < length ; i++)
				{
					var unitSettingInfo = list[i];
					FireunitData fireunitData = new FireunitData(){
						FactionIndex = unitSettingInfo.FactionIndex,
						TeamIndex = unitSettingInfo.TeamIndex,
						UnitIndex = unitSettingInfo.UnitIndex,
					};
					CreateSettingInfo createSettingInfo = new CreateSettingInfo(GamePlaySetting, fireunitData);
					characterSettingList.Enqueue(createSettingInfo);
				}
			}
			async Awaitable CreateCharacterList()
			{
				if(characterSettingList == null) return;

				int count = characterSettingList.Count;
				while(characterSettingList.Count > 0)
				{
					var item = characterSettingList.Dequeue();
					Create(item);
				}
				async void Create(CreateSettingInfo createSettingInfo)
				{
					CharacterObject characterObject = await InstantiateCharacterObject(createSettingInfo);
					count--;
				}
				while(count>0)
				{
					await Awaitable.NextFrameAsync();
				}
			}
		}

		private async Awaitable<CharacterObject> InstantiateCharacterObject(CreateSettingInfo createSettingInfo)
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
			createSettingInfo.WaitEndSetup(() => characterObject.ThisContainer.RemoveData(createSettingInfo));

			if(characterObject.ThisContainer.TryGetData<CharacterData>(out var _))
			{
				characterObject.ThisContainer.RemoveData<CharacterData>();
			}
			if(characterObject.ThisContainer.TryGetData<CreateSettingInfo>(out var _))
			{
				characterObject.ThisContainer.RemoveData<CreateSettingInfo>();
			}
			if(!characterObject.ThisContainer.TryGetData<WeaponData>(out var _))
			{
				characterObject.ThisContainer.RemoveData<WeaponData>();
			}

			CharacterData characterData = new CharacterData(){
				FactionIndex = createSettingInfo.unitSettingInfo.FactionIndex,
				TeamIndex = createSettingInfo.unitSettingInfo.TeamIndex,
				UnitIndex = createSettingInfo.unitSettingInfo.UnitIndex,
				CharacterResourcesKey = createSettingInfo.characterSettingInfo.ResourcesSetter.CharacterResourcesKey,
				WeaponResourcesKey = createSettingInfo.characterSettingInfo.ResourcesSetter.WeaponResourcesKey,
			};
			WeaponData weaponData = new WeaponData(){
				MaxAttackCount = 10,
			};

			characterObject.ThisContainer.AddDatas(characterData, createSettingInfo, weaponData);

			characterObject.UpdateObjectName();
			createSettingInfo.EndSetupCharacter();

			characterObject.gameObject.SetActive(true);
			return characterObject;
		}
	}
}
