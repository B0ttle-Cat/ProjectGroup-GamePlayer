using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class CreateCharacterObject : ComponentBehaviour, IStartSetup
	{
		[SerializeField]
		private CharacterObject characterPrefab;

		private Queue<StartUnitSettingCharacter> characterSettingDatas;

		[ShowInInspector, ReadOnly]
		public bool IsCompleteSetting { get; private set; }
		public StartUnitSetting UnitSetting { get; internal set; }


		public override void BaseAwake()
		{
			base.BaseAwake();

			if(characterPrefab != null)
				characterPrefab.gameObject.SetActive(false);
		}
		public override void BaseEnable()
		{
			IsCompleteSetting = false;
			characterSettingDatas = new Queue<StartUnitSettingCharacter>(UnitSetting.characterDatas);

			if(characterPrefab == null)
			{
				OnStopSetting();
				return;
			}
			if(UnitSetting == null || UnitSetting.characterDatas == null)
			{
				OnStopSetting();
				return;
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

			int length = characterSettingDatas.Count;
			if(length > 0)
			{
				CharacterObject characterObject = InstantiateCharacterObject(characterSettingDatas.Dequeue());
				characterObject.gameObject.SetActive(true);
				return;
			}

			IsCompleteSetting = true;
		}



		private CharacterObject InstantiateCharacterObject(StartUnitSettingCharacter characterSettingData)
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
			characterData.CharacterKey = characterSettingData.CharacterKey;
			characterData.WeaponeKey = characterSettingData.WeaponeKey;
			characterObject.UpdateObjectName();

			return characterObject;
		}
	}
}
