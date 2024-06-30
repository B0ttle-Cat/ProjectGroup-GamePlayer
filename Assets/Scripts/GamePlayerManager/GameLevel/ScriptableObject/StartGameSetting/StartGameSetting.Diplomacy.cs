//using System;
//using System.Collections;
//using System.Collections.Generic;

//using BC.OdccBase;

//using Sirenix.OdinInspector;

//using UnityEngine;

//namespace BC.GamePlayerManager
//{
//	public partial class StartGameSetting//.Diplomacy : MonoBehaviour
//	{
//		[Serializable]
//		public partial struct DiplomacyItem
//		{
//			[ValueDropdown("ShowFactionList")]
//			[TableColumnWidth(100, false)]
//			[SerializeField, ReadOnly]
//			private int factionActor;

//			[ValueDropdown("ShowFactionList")]
//			[TableColumnWidth(100, false)]
//			[SerializeField, ReadOnly]
//			private int factionTarget;

//			[SerializeField, EnumToggleButtons, DisableIf("IsEqualFaction")]
//			private FactionDiplomacyType factionDiplomacy;

//			public int FactionActor { get => factionActor; set => factionActor=value; }
//			public int FactionTarget { get => factionTarget; set => factionTarget=value; }
//			public FactionDiplomacyType FactionDiplomacy { get => factionDiplomacy; set => factionDiplomacy=value; }
//		}

//#if UNITY_EDITOR
//		[PropertyOrder(-1)]
//		[TabGroup("Tap", nameof(DiplomacyList))]
//		[Button("Fill Diplomacy List")]
//		public void FillDiplomacyList()
//		{
//			if(unitSetting == null) return;
//			DiplomacyList ??= new List<DiplomacyItem>();
//			var list = DiplomacyList;
//			foreach(var actor in unitSetting.characterDatas)
//			{
//				foreach(var target in unitSetting.characterDatas)
//				{
//					int actorIndex = actor.FactionIndex;
//					int targetIndex = target.FactionIndex;
//					bool isEqualFaction = actorIndex == targetIndex;

//					int findItemIndex = list.FindIndex(item
//						=> item.FactionActor == actorIndex
//						&& item.FactionTarget == targetIndex);

//					if(findItemIndex < 0)
//					{
//						list.Add(new() {
//							FactionActor = actorIndex,
//							FactionTarget = targetIndex,
//							FactionDiplomacy = isEqualFaction ? FactionDiplomacyType.Equal_Faction : FactionDiplomacyType.Neutral_Faction,
//						});
//					}
//					else
//					{
//						if(isEqualFaction)
//						{
//							var item = list[findItemIndex];
//							item.FactionDiplomacy = FactionDiplomacyType.Equal_Faction;
//							list[findItemIndex] = item;
//						}
//					}
//				}
//			}
//			SortCharacterDatas();

//			void SortCharacterDatas()
//			{
//				if(list != null && list.Count > 0)
//				{
//					list.Sort((a, b) => {
//						int compare = a.FactionActor.CompareTo(b.FactionActor);
//						if(compare != 0) return compare;
//						compare = a.FactionTarget.CompareTo(b.FactionTarget);
//						return compare;
//					});
//				}
//			}
//		}

//		public partial struct DiplomacyItem : IShowFactionList
//		{
//			public IEnumerable ShowFactionList()
//			{
//				return (this as IShowFactionList)._ShowFactionList();
//			}
//			public bool IsEqualFaction()
//			{
//				return factionActor == factionTarget;
//			}
//		}
//#endif
//	}
//}
