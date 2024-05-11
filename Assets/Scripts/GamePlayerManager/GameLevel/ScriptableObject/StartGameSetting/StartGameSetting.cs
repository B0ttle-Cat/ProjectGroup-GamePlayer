using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "StartGameSetting", menuName = "BC/StartSetting/new StartGameSetting/")]
	public partial class StartGameSetting : ScriptableObject
	{
#if UNITY_EDITOR
		[AssetSelector, ValidateInput("@mapSetting != null", "Is Must Not Null")]
		[PropertyOrder(-99)]
#endif
		public StartMapSetting mapSetting;
#if UNITY_EDITOR
		[AssetSelector, ValidateInput("@unitSetting != null", "Is Must Not Null")]
		[PropertyOrder(-99)]
#endif
		public StartUnitSetting unitSetting;


		[Serializable]
		public partial struct SpawnAnchor
		{
			[ValueDropdown("ShowFactionList")]
			[TableColumnWidth(150, false)]
			[ReadOnly]
			public int factionIndex;
			[ValueDropdown("ShowTeamList")]
			[TableColumnWidth(100, false)]
			[ReadOnly]
			public int teamIndex;
			[ValueDropdown("ShowAnchorList")]
			public int anchorIndex;
		}
		[TableList]
		[PropertyOrder(-97)]
		public List<SpawnAnchor> SpawnList;
	}
}
