using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class CharacterSetting : SubPlaySetting
	{
#if UNITY_EDITOR
		[HideLabel, SerializeField, FoldoutGroup("기본 세팅 값")]
		private CharacterSettingInfo DefaultSetting;
#endif
		[Space(10), ListDrawerSettings(CustomAddFunction ="CharacterSetList_CustomAddFunction")]
		public List<CharacterSettingInfo> characterSettingList;
#if UNITY_EDITOR
		private int CharacterSetList_CustomAddFunction()
		{
			try
			{
				CharacterSettingInfo addDefaultSetter = JsonUtility.FromJson<CharacterSettingInfo>(JsonUtility.ToJson(DefaultSetting));
				addDefaultSetter.characterName = $"New Character {characterSettingList.Count: 00}";
				characterSettingList.Add(addDefaultSetter);
			}
			catch(System.Exception ex)
			{
				Debug.LogException(ex);
			}
			return characterSettingList.Count;
		}
#endif

		public bool TryGetCharacterSetter(UnitSettingInfo unitSettingInfo, out CharacterSettingInfo characterSetter)
		{
			characterSetter = default;
			if(mainPlaySetting == null) return false;
			var UnitSetting  = mainPlaySetting.UnitSetting;

			if(characterSettingList == null || characterSettingList.Count == 0)
			{
				return false;
			}

			int characterSetterIndex = unitSettingInfo.CharacterSetterIndex;
			if(characterSetterIndex<0) return false;

			var charList = characterSettingList;
			int count = charList.Count;
			if(characterSetterIndex >= count) return false;

			characterSetter = charList[characterSetterIndex];
			return true;
		}
	}
}
