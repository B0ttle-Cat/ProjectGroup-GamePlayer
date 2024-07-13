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
		[TableColumnWidth(100), SerializeField, ValueDropdown("ShowTargetCharacterResourcesCard",NumberOfItemsBeforeEnablingSearch = 0)]
		private CharacterResourcesCard characterResourcesCard;

		[TableColumnWidth(50), SerializeField, ValueDropdown("ShowTargetFactionName")]
		[GUIColor("DoubleColor")]
		private int factionIndex;

		[TableColumnWidth(10), SerializeField, ValueDropdown("ShowTargetTeamIndex")]
		[GUIColor("DoubleColor")]
		private int teamIndex;

		[TableColumnWidth(10), SerializeField, ValueDropdown("ShowTargetUnitIndex")]
		[GUIColor("DoubleColor")]
		private int unitIndex;


		public int UnitIndex { get => unitIndex; set => unitIndex=value; }
		public int TeamIndex { get => teamIndex; set => teamIndex=value; }
		public int FactionIndex { get => factionIndex; set => factionIndex=value; }

		public ResourcesKey CharacterKey { get => characterResourcesCard == null ? default : characterResourcesCard.CharacterKey; }
		public ResourcesKey WeaponeKey { get => characterResourcesCard == null ? default : characterResourcesCard.WeaponeKey; }
	}
}
