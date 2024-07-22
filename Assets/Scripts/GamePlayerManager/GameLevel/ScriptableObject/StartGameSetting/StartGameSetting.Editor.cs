#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;

using Sirenix.OdinInspector;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace BC.GamePlayerManager
{
	public partial class StartGameSetting//.Editor
	{
		[Serializable]
		public partial struct GizmosInfo
		{
			[ValueDropdown("ShowFactionList")]
			[TableColumnWidth(150, false)]
			[SerializeField, ReadOnly]
			private int factionIndex;
			[ValueDropdown("ShowTeamList")]
			[TableColumnWidth(100, false)]
			[SerializeField, ReadOnly]
			private int teamIndex;

			[InlineButton("RandomColor", "", Icon = SdfIconType.Dice6Fill)]
			public Color gizmoColor;

			public int FactionIndex { get => factionIndex; set => factionIndex=value; }
			public int TeamIndex { get => teamIndex; set => teamIndex=value; }
		}

		[PropertyOrder(99)]
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
				item.FactionIndex == character.FactionIndex &&
				item.TeamIndex == character.TeamIndex);

				if(findIndex < 0)
				{
					list.Add(new() {
						FactionIndex = character.FactionIndex,
						TeamIndex = character.TeamIndex,
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
						int compare = a.FactionIndex.CompareTo(b.FactionIndex);
						if(compare != 0) return compare;
						compare = a.TeamIndex.CompareTo(b.TeamIndex);
						return compare;
					});
				}
			}
		}


		public IEnumerable ShowTargetFactionName()
		{
			var result = new ValueDropdownList<int>();
			var list = factionSetting?.factionInfoList.Select(s => (s.FactionName, s.FactionIndex));
			if(list == null) return result;

			foreach(var item in list)
			{
				result.Add(item.FactionName, item.FactionIndex);
			}
			return result;
		}

		public interface IShowFactionList
		{
			public IEnumerable _ShowFactionList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartFactionSetting thisSetting) return list;
				if(thisSetting.factionInfoList == null) return list;

				try
				{
					var factionList = thisSetting.factionInfoList;

					foreach(StartFactionSetting.FactionSettingInfo item in factionList)
					{
						list.Add($"{item.FactionIndex:00}:{item.FactionName}", item.FactionIndex);
					}
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
					list = new ValueDropdownList<int>();
				}
				return list;
			}
		}
		public interface IShowTeamList
		{
			public int FactionIndex { get; set; }
			public IEnumerable _ShowTeamList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartGameSetting thisSetting) return list;
				if(thisSetting.unitSetting == null) return list;

				int thisFactionIndex = FactionIndex;
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
		}
		public interface IShowUnitList
		{
			public int FactionIndex { get; set; }
			public int TeamIndex { get; set; }
			public IEnumerable _ShowUnitList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartGameSetting thisSetting) return list;
				if(thisSetting.unitSetting == null) return list;

				int thisFactionIndex = FactionIndex;
				int thisTeamIndex = TeamIndex;
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
		}
		public interface IShowAnchorList
		{
			public IEnumerable _ShowAnchorList()
			{
				var list = new ValueDropdownList<int>();
				Object selectedObject = Selection.activeObject;
				if(selectedObject ==null) return list;
				if(selectedObject is not StartGameSetting thisSetting) return list;
				if(thisSetting.mapSetting == null) return list;

				try
				{
					var anchorList = thisSetting.mapSetting.playMapStageInfo.anchorInfos;
					foreach(var item in anchorList)
					{
						list.Add(item.anchorName, item.anchorIndex);
					}
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
					list = new ValueDropdownList<int>();
				}
				return list;
			}
		}
		public partial struct GizmosInfo : IShowFactionList, IShowTeamList, IShowUnitList
		{
			public IEnumerable ShowFactionList()
			{
				return (this as IShowFactionList)._ShowFactionList();
			}
			public IEnumerable ShowTeamList()
			{
				return (this as IShowTeamList)._ShowTeamList();
			}
			public IEnumerable ShowUnitList()
			{
				return (this as IShowUnitList)._ShowUnitList();
			}
			private void RandomColor()
			{
				gizmoColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
			}
		}

	}
}
#endif