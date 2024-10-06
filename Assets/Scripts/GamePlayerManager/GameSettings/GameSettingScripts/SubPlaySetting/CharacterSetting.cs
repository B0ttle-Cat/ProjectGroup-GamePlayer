using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
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

		public bool TryGetCharacterSetter(Vector3Int MemberUniqueID, out CharacterSettingInfo characterSetter)
		{
			characterSetter = default;
			if(mainPlaySetting == null) return false;
			var UnitSetting  = mainPlaySetting.UnitSetting;

			if(UnitSetting == null || UnitSetting.unitSettingList == null || UnitSetting.unitSettingList.Count == 0)
			{
				return false;
			}
			if(characterSettingList == null || characterSettingList.Count == 0)
			{
				return false;
			}

			var unitList = UnitSetting.unitSettingList;
			int findIndex = unitList.FindIndex((UnitSettingInfo i) => (i.MemberUniqueID == MemberUniqueID));
			if(findIndex<0) return false;
			var find = unitList[findIndex];

			int characterSetterIndex = find.CharacterSetterIndex;
			if(characterSetterIndex<0) return false;

			var charList = characterSettingList;
			int count = charList.Count;
			if(characterSetterIndex <= count) return false;

			characterSetter = charList[characterSetterIndex];
			return true;
		}
	}
}
