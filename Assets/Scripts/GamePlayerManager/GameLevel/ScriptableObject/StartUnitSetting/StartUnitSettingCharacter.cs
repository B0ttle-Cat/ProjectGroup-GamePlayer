using System;

using BC.Base;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[Serializable]
	public partial struct StartUnitSettingCharacter : IFireunitData, ICharacterModelData
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
	}
}
