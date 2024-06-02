#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace BC.GamePlayerManager
{
	public partial class StartGameSetting : ScriptableObject//.Editor
	{
		[PropertyOrder(-98)]
		[TabGroup("Tap", nameof(TeamGizmosInfo))]
		[Button("Set Team Gizmos Color")]
		public void SetTeamGizmosColor()
		{
			if(unitSetting == null) return;
			TeamGizmosInfo ??= new System.Collections.Generic.List<GizmosInfo>();
			var list = TeamGizmosInfo;

			foreach(var character in unitSetting.characterDatas)
			{
				int findIndex = list.FindIndex(item =>
				item.factionIndex == character.FactionIndex &&
				item.teamIndex == character.TeamIndex);

				if(findIndex < 0)
				{
					list.Add(new() {
						factionIndex = character.FactionIndex,
						teamIndex = character.TeamIndex,
						gizmoColor = Color.clear,
					});
				}
			}
			SortCharacterDatas();

			void SortCharacterDatas()
			{
				if(list != null && list.Count > 0)
				{
					list.Sort((a, b) => {
						int compare = a.factionIndex.CompareTo(b.factionIndex);
						if(compare != 0) return compare;
						compare = a.teamIndex.CompareTo(b.teamIndex);
						return compare;
					});
				}
			}
		}


		public partial struct GizmosInfo
		{
#if UNITY_EDITOR
			public IEnumerable ShowFactionList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartGameSetting thisSetting) return list;
				if(thisSetting.unitSetting == null) return list;

				try
				{
					var characters = thisSetting.unitSetting.characterDatas;
					var factionList = characters.GroupBy(i => i.FactionIndex)
						.Select(i => i.First().FactionIndex);


					var table = DiplomacyTable.SelectDiplomacyTable;
					foreach(var item in factionList)
					{
						var findIndex = table.ItemList.FindIndex(i => i.FactionIndex == item);
						if(findIndex >= 0)
						{
							var diplomacy = table.ItemList[findIndex];
							list.Add($"Faction {diplomacy.FactionName}", diplomacy.FactionIndex);
						}
						else
						{
							list.Add($"Faction {item}", item);
						}
					}
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
					list = new ValueDropdownList<int>();
				}
				return list;
			}
			public IEnumerable ShowTeamList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartGameSetting thisSetting) return list;
				if(thisSetting.unitSetting == null) return list;

				int thisFactionIndex = factionIndex;
				if(thisFactionIndex < 0) return list;
				try
				{

					var characters = thisSetting.unitSetting.characterDatas;
					var teamList = characters.Where(i => i.FactionIndex == thisFactionIndex)
						.Select(i=>i.TeamIndex);


					foreach(var item in teamList)
					{
						list.Add($"Team {item}", item);
					}
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
					list = new ValueDropdownList<int>();
				}
				return list;
			}
			public IEnumerable ShowUnitList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartGameSetting thisSetting) return list;
				if(thisSetting.unitSetting == null) return list;

				int thisFactionIndex = factionIndex;
				int thisTeamIndex = teamIndex;
				if(thisFactionIndex < 0 || thisTeamIndex < 0) return list;
				try
				{

					var characters = thisSetting.unitSetting.characterDatas;
					var teamList = characters.Where(i => i.FactionIndex == thisFactionIndex && i.TeamIndex == thisTeamIndex)
						.Select(i=>i.UnitIndex);


					foreach(var item in teamList)
					{
						list.Add($"Unit {item}", item);
					}
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
					list = new ValueDropdownList<int>();
				}
				return list;
			}
			private void RandomColor()
			{
				gizmoColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
			}
#endif
		}
	}
}
#endif