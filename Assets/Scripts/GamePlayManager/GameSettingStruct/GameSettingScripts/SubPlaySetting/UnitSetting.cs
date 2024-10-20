using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class UnitSetting : SubPlaySetting
	{
		// 유닛 설정 리스트로, 중복된 MemberUniqueID가 있을 경우 오류 메시지를 표시합니다.
		[InfoBox("Duplicate found in the list.", InfoMessageType.Error, "HasDuplicate")]
		[ListDrawerSettings(CustomAddFunction = "OnAddSetting")]
		public List<UnitSettingInfo> unitSettingList;
#if UNITY_EDITOR
		private UnitSettingInfo OnAddSetting()
		{
			// 새로운 유닛 설정 항목을 생성합니다.
			UnitSettingInfo newUnitSetting = new UnitSettingInfo();

			// 현재 리스트에서 마지막 유닛 인덱스를 가져와 +1하여 새로운 유닛 설정에 할당합니다.
			int lastUnitIndex = unitSettingList.Count > 0 ? unitSettingList[^1].UnitIndex + 1 : 0;
			int lastTeamIndex = unitSettingList.Count > 0 ? unitSettingList[^1].TeamIndex : 0;
			int lastFactIndex = unitSettingList.Count > 0 ? unitSettingList[^1].FactionIndex : 0;
			newUnitSetting.UnitIndex = lastUnitIndex;
			newUnitSetting.TeamIndex = lastTeamIndex;
			newUnitSetting.FactionIndex = lastFactIndex;

			// 새로운 유닛 설정 항목을 반환합니다.
			return newUnitSetting;
		}
		private bool HasDuplicate() => unitSettingList != null && unitSettingList.GroupBy(u => u.MemberUniqueID).Any(g => g.Count() > 1);
		[Button, PropertyOrder(-1)]
		private void SortList()
		{
			if(unitSettingList == null) return;
			unitSettingList = unitSettingList
				.OrderBy(u => u.FactionIndex) // FactionIndex 기준으로 먼저 정렬
				.ThenBy(u => u.TeamIndex)     // 그 다음 TeamIndex 기준으로 정렬
				.ThenBy(u => u.UnitIndex)     // 마지막으로 UnitIndex 기준으로 정렬
				.ToList();
		}
#endif
	}
	[Serializable]
	public struct UnitSettingInfo : IFireunitIndex
	{
#if UNITY_EDITOR
		[HideLabel, ShowInInspector, GUIColor("GetColor"), DisplayAsString, EnableGUI, PropertyOrder(-10)]
		string colortitle => "█ Unit Setting";
#endif
		[SerializeField, CustomValueDrawer("MemberUniqueID_CustomDrawer")]
		private Vector3Int fireunitData; // 파벌, 팀, 유닛 정보를 저장하는 Vector3Int (x = FactionIndex, y = TeamIndex, z = UnitIndex)
		[SerializeField, HideLabel, ValueDropdown("CharacterSetterIndex_ValueDropdown")]
		private int characterSetterIndex; // 캐릭터 설정의 인덱스를 저장

		// 유닛의 고유 ID를 가져오는 속성
		public Vector3Int MemberUniqueID => fireunitData;

		// fireunitData의 개별 구성 요소에 접근하기 위한 속성들
		public int FactionIndex { get => fireunitData.x; set => fireunitData.x = value; } // 유닛이 속한 파벌을 나타냄
		public int TeamIndex { get => fireunitData.y; set => fireunitData.y = value; }     // 파벌 내 팀을 나타냄
		public int UnitIndex { get => fireunitData.z; set => fireunitData.z = value; }     // 팀 내 유닛 인덱스를 나타냄
		public int CharacterSetterIndex { get => characterSetterIndex; set => characterSetterIndex = value; } // 캐릭터 설정 인덱스
		public void Dispose() { } // 필요할 경우 정리 작업을 위한 비어있는 메서드

#if UNITY_EDITOR
		private Color GetColor()
		{
			if(UnityEditor.Selection.activeObject is not UnitSetting setting) return Color.clear;
			if(setting.mainPlaySetting == null) return Color.clear;
			if(setting.mainPlaySetting.FactionSetting == null) return Color.clear;
			List<FactionSettingInfo> dataList = setting.mainPlaySetting.FactionSetting.factionSettingList;
			if(dataList == null) return Color.clear;
			int find = FactionIndex;
			FactionSettingInfo factionInfo = dataList.FirstOrDefault(f => f.FactionIndex == find);
			return factionInfo.FactionColor;
		}

		// Unity Inspector에서 캐릭터 설정 인덱스를 위한 드롭다운 리스트를 제공합니다.
		private IEnumerable CharacterSetterIndex_ValueDropdown()
		{
			ValueDropdownList<int> list = new ValueDropdownList<int>();
			if(UnityEditor.Selection.activeObject is not UnitSetting setting) return list;
			if(setting.mainPlaySetting == null) return list;
			if(setting.mainPlaySetting.CharacterSetting == null) return list;
			var dataList = setting.mainPlaySetting.CharacterSetting.characterSettingList;
			if(dataList == null) return list;

			int length = dataList.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var characterSetter = dataList[i];
				list.Add(characterSetter.characterName, i); // 드롭다운 리스트에 캐릭터 이름과 인덱스를 추가
			}

			return list;
		}

		// MemberUniqueID를 Unity Inspector에서 드롭다운으로 표시하는 커스텀 드로어
		private void MemberUniqueID_CustomDrawer()
		{
			if(UnityEditor.Selection.activeObject is not UnitSetting setting) return;
			if(setting.mainPlaySetting == null) return;

			UnityEditor.EditorGUILayout.BeginHorizontal();
			Vector3Int unitData = fireunitData;
			if(DrawFaction()) if(DrawTeam()) DrawUnit();
			fireunitData = unitData;
			UnityEditor.EditorGUILayout.EndHorizontal();

			bool DrawFaction()
			{
				if(setting.mainPlaySetting.FactionSetting == null) return false;
				List<FactionSettingInfo> factionDataList = setting.mainPlaySetting.FactionSetting.factionSettingList;
				if(factionDataList == null || factionDataList.Count == 0) return false;

				unitData.x = Sirenix.Utilities.Editor.SirenixEditorFields.Dropdown<int>(unitData.x,
					factionDataList.Select(i => i.FactionIndex).ToArray(),
					factionDataList.Select(i => $"{(string.IsNullOrWhiteSpace(i.FactionName) ? i.FactionIndex : i.FactionName)}").ToArray());
				return true;
			}
			bool DrawTeam()
			{
				if(setting.mainPlaySetting.TeamSetting == null) return false;
				List<TeamSettingInfo> teamDataList = setting.mainPlaySetting.TeamSetting.teamSettingList;
				if(teamDataList == null || teamDataList.Count == 0) return false;

				unitData.y = Sirenix.Utilities.Editor.SirenixEditorFields.Dropdown<int>(unitData.y,
					teamDataList.Where(i => i.FactionIndex == unitData.x).Select(i => i.TeamIndex).ToArray(),
					teamDataList.Where(i => i.FactionIndex == unitData.x).Select(i => $"Team: {i.TeamIndex:00}").ToArray());
				return true;
			}
			bool DrawUnit()
			{
				unitData.z = Sirenix.Utilities.Editor.SirenixEditorFields.Dropdown<int>(unitData.z,
					Enumerable.Range(0, 20).ToArray(),
					Enumerable.Range(0, 20).Select(i => $"Unit: {i:00}").ToArray());
				return true;
			}

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
