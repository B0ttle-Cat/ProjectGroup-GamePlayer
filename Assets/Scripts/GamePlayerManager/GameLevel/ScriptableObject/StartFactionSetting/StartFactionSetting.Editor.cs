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
	public partial class StartFactionSetting : IConnectStartGameSetting_Editor//.Editor ,
	{
		[ShowInInspector, ReadOnly, PropertyOrder(-999)]
		public StartGameSetting startGameSetting { get; set; }
		public void ConnectStartGameSetting(StartGameSetting startGameSetting)
		{
			this.startGameSetting = startGameSetting;
		}

		public partial struct DiplomacySettingInfo : IShowFactionList
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

					foreach(FactionSettingInfo item in factionList)
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

			diplomacyInfoList ??= new List<DiplomacySettingInfo>();
			var list = diplomacyInfoList;
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
						// FactionActor�� �������� ù ��° ����
						int compare = a.FactionActor.CompareTo(b.FactionActor);
						if(compare != 0) return compare;

						// FactionActor�� FactionTarget�� ���� ���, �̸� ���� �켱�Ͽ� ó��
						bool isAEqual = a.FactionActor == a.FactionTarget;
						bool isBEqual = b.FactionActor == b.FactionTarget;
						if(isAEqual && !isBEqual) return -1; // a�� ���� b�� �ٸ��� a�� ����
						if(!isAEqual && isBEqual) return 1;  // b�� ���� a�� �ٸ��� b�� ����

						// FactionActor�� ���� ���, FactionTarget�� �������� �� ��° ����
						return a.FactionTarget.CompareTo(b.FactionTarget);
					});

				}
			}
		}



		Color prevColor;
		Color prevBGColor;
		Color prevContentColor;
		private void DrawOnBeginListElementGUI(int index)
		{
			if(diplomacyInfoList == null) return;
			prevColor = GUI.color;
			prevBGColor = GUI.backgroundColor;
			prevContentColor = GUI.contentColor;

			int length = diplomacyInfoList.Count;
			var thisItem = diplomacyInfoList[index];
			if(thisItem.FactionActor == thisItem.FactionTarget)
			{
				GUI.backgroundColor = Color.gray;
				GUI.contentColor = Color.black;
			}
			else
			{
				if(thisItem.FactionActor > thisItem.FactionTarget)
				{
					GUI.backgroundColor = Color.gray;
					GUI.contentColor = Color.gray;
					for(int i = 0 ; i < index+1 ; i++)
					{
						var checkItem = diplomacyInfoList[i];
						if(thisItem.FactionActor == checkItem.FactionTarget && thisItem.FactionTarget == checkItem.FactionActor)
						{
							thisItem.FactionDiplomacy = checkItem.FactionDiplomacy;
							diplomacyInfoList[index] = thisItem;
							break;
						}
					}
				}
				//bool check = true;
				//for(int i = 0 ; i < length ; i++)
				//{
				//	if(i == index) continue;
				//	var checkItem = diplomacyInfoList[i];
				//
				//	if(thisItem.FactionActor == checkItem.FactionTarget && thisItem.FactionTarget == checkItem.FactionActor)
				//	{
				//		if(thisItem.FactionDiplomacy != checkItem.FactionDiplomacy)
				//		{
				//			check = false;
				//			break;
				//		}
				//	}
				//}
				//if(!check)
				//{
				//	GUI.contentColor = Color.red;
				//}
			}
		}
		private void DrawOnEndListElementGUI(int index)
		{
			GUI.color = prevColor;
			GUI.backgroundColor = prevBGColor;
			GUI.contentColor = prevColor;
		}
	}
}
#endif