using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class CharacterSetting : SubPlaySetting
	{
#if UNITY_EDITOR
		[HideLabel, SerializeField, FoldoutGroup("기본 세팅 값")]
		private CharacterSetter DefaultSetter;
#endif
		[Space(10), ListDrawerSettings(CustomAddFunction ="CharacterSetList_CustomAddFunction")]
		public List<CharacterSetter> characterSetterList;
#if UNITY_EDITOR
		private int CharacterSetList_CustomAddFunction()
		{
			try
			{
				CharacterSetter addDefaultSetter = JsonUtility.FromJson<CharacterSetter>(JsonUtility.ToJson(DefaultSetter));
				addDefaultSetter.characterName = $"New Character {characterSetterList.Count: 00}";
				characterSetterList.Add(addDefaultSetter);
			}
			catch(System.Exception ex)
			{
				Debug.LogException(ex);
			}
			return characterSetterList.Count;
		}
#endif
	}
}
