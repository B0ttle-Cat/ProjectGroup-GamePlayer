using System;
using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class TeamSetting : SubPlaySetting
	{
		[InfoBox("Duplicate found in the list.", InfoMessageType.Error, "HasDuplicate")]
		[ListDrawerSettings(CustomAddFunction = "OnAddSetting")]
		public List<TeamSettingInfo> teamSettingList;
#if UNITY_EDITOR
		private TeamSettingInfo OnAddSetting()
		{
			// 새로운 유닛 설정 항목을 생성합니다.
			TeamSettingInfo newSetting = new TeamSettingInfo();

			// 현재 리스트에서 마지막 유닛 인덱스를 가져와 +1하여 새로운 유닛 설정에 할당합니다.
			int lastTeamIndex = teamSettingList.Count > 0 ? teamSettingList[^1].TeamIndex + 1 : 0;
			int lastFactIndex = teamSettingList.Count > 0 ? teamSettingList[^1].FactionIndex : 0;
			newSetting.TeamIndex = lastTeamIndex;
			newSetting.FactionIndex = lastFactIndex;

			// 새로운 유닛 설정 항목을 반환합니다.
			return newSetting;
		}
		// unitSettingList에 중복된 MemberUniqueID가 있는지 확인하는 메서드
		private bool HasDuplicate() => teamSettingList != null && teamSettingList.GroupBy(u => u.TeamUniqueID).Any(g => g.Count() > 1);

		[Button, PropertyOrder(-1)]
		private void SortList()
		{
			if(teamSettingList == null) return;
			teamSettingList = teamSettingList
				.OrderBy(u => u.FactionIndex) // FactionIndex 기준으로 먼저 정렬
				.ThenBy(u => u.TeamIndex)     // 그 다음 TeamIndex 기준으로 정렬
				.ToList();
		}
#endif
	}
	[Serializable]
	public partial struct TeamSettingInfo : IFireteamData
	{
#if UNITY_EDITOR
		[HideLabel, ShowInInspector, GUIColor("GetColor"), DisplayAsString, EnableGUI, PropertyOrder(-10)]
		string colortitle => "█ Team Setting";
#endif
		[SerializeField, CustomValueDrawer("MemberUniqueID_CustomDrawer")]
		private Vector2Int teamUniqueID; // 파벌, 팀, 정보를 저장하는 Vector3Int (x = FactionIndex, y = TeamIndex)
		[SerializeField, LabelText("Anchor"),LabelWidth(40), ValueDropdown("AnchorIndex_ValueDropdown")]
		private int anchorIndex;

		public Vector2Int TeamUniqueID { get => teamUniqueID; set => teamUniqueID=value; }
		public int FactionIndex { get => teamUniqueID.x; set => teamUniqueID.x=value; }
		public int TeamIndex { get => teamUniqueID.y; set => teamUniqueID.y=value; }
		public int AnchorIndex { get => anchorIndex; set => anchorIndex=value; }

		public void Dispose()
		{
		}


#if UNITY_EDITOR
		private Color GetColor()
		{
			if(UnityEditor.Selection.activeObject is not TeamSetting setting) return Color.clear;
			if(setting.mainPlaySetting == null) return Color.clear;
			if(setting.mainPlaySetting.FactionSetting == null) return Color.clear;
			List<FactionSettingInfo> dataList = setting.mainPlaySetting.FactionSetting.factionSettingList;
			if(dataList == null) return Color.clear;
			int find = FactionIndex;
			FactionSettingInfo factionInfo = dataList.FirstOrDefault(f => f.FactionIndex == find);
			return factionInfo.FactionColor;
		}

		// Unity Inspector에서 앵커 설정 인덱스를 위한 드롭다운 리스트를 제공합니다.
		private System.Collections.IEnumerable AnchorIndex_ValueDropdown()
		{
			ValueDropdownList<int> list = new ValueDropdownList<int>();
			if(UnityEditor.Selection.activeObject is not TeamSetting setting) return list;
			if(setting.mainPlaySetting == null) return list;
			if(setting.mainPlaySetting.MapSetting == null) return list;
			var dataList = setting.mainPlaySetting.MapSetting.playMapStageInfo.anchorInfos;
			if(dataList == null) return list;

			int length = dataList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var dara = dataList[i];
				list.Add($"{dara.anchorIndex:00} : ({dara.anchorName})", dara.anchorIndex);
			}

			return list;
		}

		// MemberUniqueID를 Unity Inspector에서 드롭다운으로 표시하는 커스텀 드로어
		private void MemberUniqueID_CustomDrawer()
		{
			if(UnityEditor.Selection.activeObject is not TeamSetting unitSetting) return;
			if(unitSetting.mainPlaySetting == null) return;
			if(unitSetting.mainPlaySetting.FactionSetting == null) return;
			List<FactionSettingInfo> dataList = unitSetting.mainPlaySetting.FactionSetting.factionSettingList;
			if(dataList == null) return;

			// 파벌, 팀, 유닛 드롭다운을 그리기 위한 가로 레이아웃 시작
			UnityEditor.EditorGUILayout.BeginHorizontal();

			// FactionIndex 선택을 위한 드롭다운
			teamUniqueID.x = Sirenix.Utilities.Editor.SirenixEditorFields.Dropdown<int>(teamUniqueID.x,
				dataList.Select(i => i.FactionIndex).ToArray(),
				dataList.Select(i => $"{(string.IsNullOrWhiteSpace(i.FactionName) ? i.FactionIndex : i.FactionName)}").ToArray());

			// TeamIndex 선택을 위한 드롭다운
			teamUniqueID.y = Sirenix.Utilities.Editor.SirenixEditorFields.Dropdown<int>(teamUniqueID.y,
				Enumerable.Range(0, 20).ToArray(),
				Enumerable.Range(0, 20).Select(i => $"Team: {i:00}").ToArray());

			// 가로 레이아웃 종료
			UnityEditor.EditorGUILayout.EndHorizontal();
		}

		// 0부터 19까지의 팀 인덱스를 생성하는 리스트 생성 메서드
		private List<int> GetTeamDropdownList()
		{
			List<int> teams = new List<int>();
			for(int i = 0 ; i < 20 ; i++)
			{
				teams.Add(i); // 각 팀 인덱스를 리스트에 추가
			}
			return teams;
		}

		// 0부터 19까지의 유닛 인덱스를 생성하는 리스트 생성 메서드
		private List<int> GetUnitDropdownList()
		{
			List<int> units = new List<int>();
			for(int i = 0 ; i < 20 ; i++)
			{
				units.Add(i); // 각 유닛 인덱스를 리스트에 추가
			}
			return units;
		}
#endif
	}
}
