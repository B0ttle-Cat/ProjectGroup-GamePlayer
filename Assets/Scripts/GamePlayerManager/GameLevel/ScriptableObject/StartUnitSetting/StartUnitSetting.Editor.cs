#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public partial class StartUnitSetting//.Editor
	{
		StartGameSetting editorStartGameSetting;
		public void ConnectStartGameSetting(StartGameSetting startGameSetting)
		{
			editorStartGameSetting = startGameSetting;
			if(characterDatas != null)
			{
				for(int i = 0 ; i < characterDatas.Count ; i++)
				{
					var data = characterDatas[i];
					data.editorStartGameSetting = startGameSetting;
					characterDatas[i] = data;
				}
			}
		}

		public void OnValidate()
		{
			IsDouble_CharacterDatas = CheckForDuplicates(
				characterDatas.Select(s => s as IFireunitData).ToList(),
				(int index, bool change) => characterDatas[index].SetDouble(change));
		}
		private bool IsDouble_CharacterDatas;
		private string DuplicatesMessage => $"중복된 요소가 존재합니다.";
		private bool CheckForDuplicates(List<IFireunitData> checkList, Action<int, bool> onSetDouble)
		{
			int dataCount = checkList.Count;
			bool isDuplicates = false;
			for(int i = 0 ; i < dataCount ; i++)
			{
				var data = checkList[i];
				onSetDouble?.Invoke(i, false);
				checkList[i] = data;
			}
			for(int i = 0 ; i < dataCount ; i++)
			{
				for(int ii = i ; ii < dataCount ; ii++)
				{
					if(i == ii) continue;
					var dataA = checkList[i];
					var dataB = checkList[ii];

					var unitA = dataA as IFireunitData;
					var unitB = dataB as IFireunitData;

					if(unitA.IsEqualsUnit(unitB))
					{
						isDuplicates = true;

						onSetDouble?.Invoke(i, true);
						onSetDouble?.Invoke(ii, true);

						checkList[i] = dataA;
						checkList[ii] = dataB;
					}
				}
			}
			return isDuplicates;
		}
		[PropertySpace(50)]
		[Button(ButtonHeight = (int)ButtonSizes.Large)]
		private void SortCharacterDatas()
		{
			if(characterDatas != null && characterDatas.Count > 0)
			{
				characterDatas.Sort((a, b) => {
					int compare = a.FactionIndex.CompareTo(b.FactionIndex);
					if(compare != 0) return compare;
					compare = a.TeamIndex.CompareTo(b.TeamIndex);
					if(compare != 0) return compare;
					compare = a.UnitIndex.CompareTo(b.UnitIndex);
					return compare;
				});
			}
		}
	}
	public partial struct StartUnitSettingCharacter//Editor
	{
		internal StartGameSetting editorStartGameSetting { get; set; }
		private bool IsDouble { get; set; }
		private Color DoubleColor() { return IsDouble ? Color.red : Color.white; }
		private IEnumerable ShowTargetFactionName()
		{
			return editorStartGameSetting.ShowTargetFactionName();
		}
		private IEnumerable ShowTargetTeamIndex()
		{
			var result = new ValueDropdownList<int>();
			for(int i = 0 ; i < 100 ; i++)
			{
				result.Add(i.ToString(), i);
			}
			return result;
		}
		private IEnumerable ShowTargetUnitIndex()
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
	}
}

#endif