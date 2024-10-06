using System;
using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.Map;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class CreateSettingInfo : DataObject, IFireunitData
	{
		public Vector3Int memberUniqueID;
		public FactionSettingInfo factionSettingInfo;
		public TeamSettingInfo teamSettingInfo;
		public UnitSettingInfo unitSettingInfo;
		public CharacterSettingInfo characterSettingInfo;

		public CreateSettingInfo(MainPlaySetting gameSetting, IFireunitData fireunitData)
		{
			this.memberUniqueID = fireunitData.MemberUniqueID;
			factionSettingInfo = gameSetting.FactionSetting.factionSettingList.Find(i => fireunitData.IsEqualsFaction(i));
			teamSettingInfo = gameSetting.TeamSetting.teamSettingList.Find(i => fireunitData.IsEqualsTeam(i));
			unitSettingInfo = gameSetting.UnitSetting.unitSettingList.Find(i => fireunitData.IsEqualsUnit(i));
			characterSettingInfo = gameSetting.CharacterSetting.characterSettingList[unitSettingInfo.CharacterSetterIndex];

			endSetupFireunit =  false;
			endSetupCharacter = false;
		}

		public Vector3Int MemberUniqueID => memberUniqueID;
		public int FactionIndex { get => memberUniqueID.x; set => memberUniqueID.x = value; }
		public int TeamIndex { get => memberUniqueID.y; set => memberUniqueID.y = value; }
		public int UnitIndex { get => memberUniqueID.z; set => memberUniqueID.z = value; }



		private bool endSetupFireunit = false;
		private bool endSetupCharacter = false;
		private Action deleteAction;

		public void EndSetupFireunit()
		{
			endSetupFireunit = true;
			CheckDelete();
		}
		public void EndSetupCharacter()
		{
			endSetupCharacter = true;
			CheckDelete();
		}
		public void WaitEndSetup(Action deleteAction)
		{
			this.deleteAction = deleteAction;
			endSetupFireunit =  false;
			endSetupCharacter = false;
		}
		private void CheckDelete()
		{
			if(endSetupFireunit && endSetupCharacter)
			{
				deleteAction?.Invoke();
			}
		}
	}

	public class CreateCharacterObject : ComponentBehaviour, IStartSetup
	{

		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				if(characterPrefab == null || GamePlaySetting == null || SpawnList == null) return false;
				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}

		[SerializeField]
		private CharacterObject characterPrefab;

		public MainPlaySetting GamePlaySetting { get; internal set; }
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
				var lowLevelAIManager = await ThisContainer.AwaitGetObject<IGetLowLevelAIManager>(null, DisableCancelToken);
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
				characterObject.ThisContainer.AddData<WeaponData>();
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

			characterObject.ThisContainer.AddData(characterData);
			characterObject.ThisContainer.AddData(createSettingInfo);
			characterObject.ThisContainer.AddData(weaponData);

			characterObject.UpdateObjectName();
			createSettingInfo.EndSetupCharacter();

			characterObject.gameObject.SetActive(true);
			return characterObject;
		}
	}
}
