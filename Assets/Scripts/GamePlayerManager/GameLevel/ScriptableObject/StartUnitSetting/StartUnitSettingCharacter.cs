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

		private Vector3Int fireunitData;
		private int factionIndex;
		private int fireteamIndex;
		private int fireunitIndex;

		public Vector3Int MemberUniqueID => fireunitData;
		[TableColumnWidth(50), SerializeField, ValueDropdown("ShowTargetFactionName")]
		[GUIColor("DoubleColor")]
		public int FactionIndex { get => fireunitData.x; set => fireunitData.x = value; }

		[TableColumnWidth(10), SerializeField, ValueDropdown("ShowTargetTeamIndex")]
		[GUIColor("DoubleColor")]
		public int TeamIndex { get => fireunitData.y; set => fireunitData.y = value; }

		[TableColumnWidth(10), SerializeField, ValueDropdown("ShowTargetUnitIndex")]
		[GUIColor("DoubleColor")]
		public int UnitIndex { get => fireunitData.z; set => fireunitData.z = value; }

		public ResourcesKey CharacterResourcesKey { get => characterCard == null ? default : characterCard.CharacterResourcesKey; }
		public ResourcesKey WeaponResourcesKey { get => characterCard == null ? default : characterCard.WeaponResourcesKey; }

		public void Dispose()
		{
			CharacterResourcesKey.Dispose();
			WeaponResourcesKey.Dispose();
		}
	}
}
