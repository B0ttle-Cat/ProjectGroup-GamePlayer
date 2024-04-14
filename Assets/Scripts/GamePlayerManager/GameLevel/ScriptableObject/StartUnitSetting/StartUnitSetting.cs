using System;
using System.Collections.Generic;
using System.Linq;

using BC.GameBaseInterface;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "StartUnitSetting", menuName = "BC/new StartUnitSetting")]
	public class StartUnitSetting : ScriptableObject
	{
		[InfoBox("@DuplicatesMessage", "IsDouble_CharacterDatas", InfoMessageType = InfoMessageType.Error)]
		public List<StartUnitSettingCharacter> characterDatas;
		//[Space(50)]
		//[InfoBox("@DuplicatesMessage", "IsDouble_PositionDatas", InfoMessageType = InfoMessageType.Error)]
		//public List<StartUnitSettingPosition> positionDatas;

#if UNITY_EDITOR
		public void OnValidate()
		{
			IsDouble_CharacterDatas = CheckForDuplicates(
				characterDatas.Select(s => s as IFireunitData).ToList(),
				(int index, bool change) => characterDatas[index].SetDouble(change));

			//	IsDouble_PositionDatas = CheckForDuplicates(
			//		positionDatas.Select(s => s as IFireunitData).ToList(),
			//		(int index, bool change) => positionDatas[index].SetDouble(change));
		}
		private bool IsDouble_CharacterDatas;
		//private bool IsDouble_PositionDatas;


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
				characterDatas.Sort((a, b) =>
				{
					int compare = a.FactionIndex.CompareTo(b.FactionIndex);
					if(compare != 0) return compare;
					compare = a.TeamIndex.CompareTo(b.TeamIndex);
					if(compare != 0) return compare;
					compare = a.UnitIndex.CompareTo(b.UnitIndex);
					return compare;
				});
			}
			//if(positionDatas != null && positionDatas.Count > 0)
			//{
			//	positionDatas.Sort((a, b) =>
			//	{
			//		int compare = a.FactionIndex.CompareTo(b.FactionIndex);
			//		if(compare != 0) return compare;
			//		compare = a.TeamIndex.CompareTo(b.TeamIndex);
			//		if(compare != 0) return compare;
			//		compare = a.UnitIndex.CompareTo(b.UnitIndex);
			//		return compare;
			//	});
			//}
		}
#endif
	}
}