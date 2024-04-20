using System;
using System.Collections;

using BC.Base;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[Serializable]
	public struct StartUnitSettingCharacter : IFireunitData, ICharacterModelData
	{
		[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetFactionName")]
		[HideLabel,SuffixLabel("faction", Overlay = true)]
		[GUIColor("DoubleColor")]
		private int factionIndex;
		[HorizontalGroup,SerializeField, ValueDropdown("ShowTargetTeamIndex")]
		[HideLabel,SuffixLabel("team", Overlay = true)]
		[GUIColor("DoubleColor")]
		private int teamIndex;
		[HorizontalGroup,SerializeField, ValueDropdown("ShowTargetUnitIndex")]
		[HideLabel,SuffixLabel("unit", Overlay = true)]
		[GUIColor("DoubleColor")]
		private int unitIndex;

		[SerializeField, HideLabel]
		[FoldoutGroup("Model Resources"), HorizontalGroup("Model Resources/Model"), BoxGroup("Model Resources/Model/Character")]
		private ResourcesKey characterKey;
		[SerializeField, HideLabel]
		[FoldoutGroup("Model Resources"), HorizontalGroup("Model Resources/Model"), BoxGroup("Model Resources/Model/Weapone")]
		private ResourcesKey weaponeKey;


		public int UnitIndex { get => unitIndex; set => unitIndex=value; }
		public int TeamIndex { get => teamIndex; set => teamIndex=value; }
		public int FactionIndex { get => factionIndex; set => factionIndex=value; }

		public ResourcesKey CharacterKey { get => characterKey; set => characterKey=value; }
		public ResourcesKey WeaponeKey { get => weaponeKey; set => weaponeKey=value; }
#if UNITY_EDITOR
		private bool IsDouble { get; set; }
		private Color DoubleColor() { return IsDouble ? Color.red : Color.white; }
		private static IEnumerable ShowTargetFactionName()
		{
			return FriendshipItem.ShowTargetFactionName();
		}
		private static IEnumerable ShowTargetTeamIndex()
		{
			var result = new ValueDropdownList<int>();
			for(int i = 0 ; i < 100 ; i++)
			{
				result.Add(i.ToString(), i);
			}
			return result;
		}
		private static IEnumerable ShowTargetUnitIndex()
		{
			var result = new ValueDropdownList<int>();
			for(int i = 0 ; i < 10 ; i++)
			{
				result.Add(i.ToString(), i);
			}
			return result;
		}
		internal void SetDouble(bool _isDouble)
		{
			IsDouble = _isDouble;
		}
#endif
	}
}
