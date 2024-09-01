using System;

using BC.Base;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[Serializable]
	public partial struct StartUnitSettingCharacter : IFireunitData
	{
		[TableColumnWidth(100), SerializeField, ValueDropdown("ShowTargetCharacterResourcesCard", NumberOfItemsBeforeEnablingSearch = 0, DropdownWidth = 300)]
		private CharacterResourcesCard characterCard;

		[TableColumnWidth(50), SerializeField, ValueDropdown("ShowTargetFactionName")]
		[GUIColor("DoubleColor")]
		private int factionIndex;

		[TableColumnWidth(10), SerializeField, ValueDropdown("ShowTargetTeamIndex")]
		[GUIColor("DoubleColor")]
		private int teamIndex;

		[TableColumnWidth(10), SerializeField, ValueDropdown("ShowTargetUnitIndex")]
		[GUIColor("DoubleColor")]
		private int unitIndex;

		public int MemberUniqueID { get => 1000000 + (FactionIndex * 010000) + (TeamIndex * 000100) + (UnitIndex); }
		public int UnitIndex { get => unitIndex; set => unitIndex = value; }
		public int TeamIndex { get => teamIndex; set => teamIndex = value; }
		public int FactionIndex { get => factionIndex; set => factionIndex = value; }

		public ResourcesKey CharacterResourcesKey { get => characterCard == null ? default : characterCard.CharacterResourcesKey; }
		public ResourcesKey WeaponResourcesKey { get => characterCard == null ? default : characterCard.WeaponResourcesKey; }

		public void Dispose()
		{
			CharacterResourcesKey.Dispose();
			WeaponResourcesKey.Dispose();
		}
	}
}
