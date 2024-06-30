#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace BC.GamePlayerManager
{
	public partial class StartFactionSetting//.Editor
	{
		StartGameSetting startGameSetting;
		public void ConnectStartGameSetting(StartGameSetting startGameSetting)
		{
			this.startGameSetting = startGameSetting;
		}

		public partial struct DiplomacyItem : IShowFactionList
		{
			private IEnumerable ShowFactionList()
			{
				return (this as IShowFactionList)._ShowFactionList();
			}
			private bool IsEqualFaction()
			{
				return FactionActor == FactionTarget;
			}
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

					foreach(FactionInfo item in factionList)
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

		[PropertyOrder(1)]
		[Button("Fill Diplomacy List")]
		public void FillDiplomacyList()
		{
			Object selectedObject = Selection.activeObject;
			if(selectedObject ==null) return;
			if(selectedObject is not StartFactionSetting thisSetting) return;
			if(thisSetting.factionInfoList == null) return;

			var infoList = thisSetting.factionInfoList;

			DiplomacyList ??= new List<DiplomacyItem>();
			var list = DiplomacyList;
			foreach(var actor in infoList)
			{
				foreach(var target in infoList)
				{
					int actorIndex = actor.FactionIndex;
					int targetIndex = target.FactionIndex;
					bool isEqualFaction = actorIndex == targetIndex;

					int findItemIndex = list.FindIndex(item
						=> item.FactionActor == actorIndex
						&& item.FactionTarget == targetIndex);

					if(findItemIndex < 0)
					{
						list.Add(new() {
							FactionActor = actorIndex,
							FactionTarget = targetIndex,
							FactionDiplomacy = isEqualFaction ? FactionDiplomacyType.Equal_Faction : FactionDiplomacyType.Neutral_Faction,
						});
					}
					else
					{
						if(isEqualFaction)
						{
							var item = list[findItemIndex];
							item.FactionDiplomacy = FactionDiplomacyType.Equal_Faction;
							list[findItemIndex] = item;
						}
					}
				}
			}
			SortCharacterDatas();


			void SortCharacterDatas()
			{
				if(list != null && list.Count > 0)
				{

					list.Sort((a, b) => {
						// FactionActor를 기준으로 첫 번째 정렬
						int compare = a.FactionActor.CompareTo(b.FactionActor);
						if(compare != 0) return compare;

						// FactionActor와 FactionTarget이 같은 경우, 이를 가장 우선하여 처리
						bool isAEqual = a.FactionActor == a.FactionTarget;
						bool isBEqual = b.FactionActor == b.FactionTarget;
						if(isAEqual && !isBEqual) return -1; // a가 같고 b가 다르면 a를 먼저
						if(!isAEqual && isBEqual) return 1;  // b가 같고 a가 다르면 b를 먼저

						// FactionActor가 같은 경우, FactionTarget을 기준으로 두 번째 정렬
						return a.FactionTarget.CompareTo(b.FactionTarget);
					});

				}
			}
		}



		Color prevColor = GUI.backgroundColor;
		private void DrawOnBeginListElementGUI(int index)
		{
			if(DiplomacyList == null) return;
			int length = DiplomacyList.Count;

			var thisItem = DiplomacyList[index];
			bool check = true;
			for(int i = 0 ; i < length ; i++)
			{
				if(i == index) continue;
				var checkItem = DiplomacyList[i];

				if(thisItem.FactionActor == checkItem.FactionTarget && thisItem.FactionTarget == checkItem.FactionActor)
				{
					if(thisItem.FactionDiplomacy != checkItem.FactionDiplomacy)
					{
						check = false;
						break;
					}
				}
			}

			prevColor = GUI.backgroundColor;
			if(!check)
			{
				GUI.backgroundColor = Color.yellow;
			}
		}
		private void DrawOnEndListElementGUI(int index)
		{
			GUI.backgroundColor = prevColor;
		}
	}
}
#endif