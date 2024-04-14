using System.Collections.Generic;

using BC.GameBaseInterface;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public enum FactionDiplomacyType : int
	{
		My_Faction = 3,
		Alliance_Faction = 2,   // 가장 낮은 점수가 50 이상
		Positive_Faction = 1,   // 가장 낮은 점수가 25 이상 & 높은 점수가 50 이상
		Neutral_Faction = 0,    // 그외
		Negative_Faction = -1, // 가장 높은 점수가 -25 이하 & 높은 점수가 -50 이하
		Enemy_Faction = -2,     // 가장 높은 점수가 -50 이하
	}

	public class DiplomacyComputer : ComponentBehaviour
	{
		private DiplomacyTable DiplomacyTable { get; set; }
		[ShowInInspector, ReadOnly, TableList]
		public List<DiplomacyItem> diplomacyItemList { get; private set; }

		private readonly int AllianceValue = 50;
		private readonly int PositiveValue = 25;
		private readonly int NegativeValue = -25;
		private readonly int EnemyValue = -50;
		private readonly int MaxDistance = 50;

		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<DiplomacyData>(out _))
			{
				ThisContainer.AddData<DiplomacyData>();
			}
		}


		public override void BaseAwake()
		{
			base.BaseAwake();

			DiplomacyTable = null;
			if(ThisObject.ThisContainer.TryGetData<DiplomacyData>(out var data))
			{
				DiplomacyTable = data.Table;
			}


			diplomacyItemList = DiplomacyTable != null ? DiplomacyTable.ItemList : new List<DiplomacyItem>();
		}


		public bool GetFaction(int factionIndex, out DiplomacyItem factionItem)
		{
			int Count = diplomacyItemList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var item = diplomacyItemList[i];
				if(item.FactionIndex == factionIndex)
				{
					factionItem = item;
					return true;
				}
			}
			factionItem = default;
			return false;
		}
		public void SetFaction(DiplomacyItem factionItem)
		{
			int targetIndex = factionItem.FactionIndex;
			if(targetIndex < 0) return;

			int Count = diplomacyItemList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var item = diplomacyItemList[i];
				if(item.FactionIndex == targetIndex)
				{
					diplomacyItemList[i] = factionItem;
					return;
				}
			}
			diplomacyItemList.Add(factionItem);
		}

		public FactionDiplomacyType GetFactionDiplomacyType(int factionA, int factionB)
		{
			if(factionA == factionB)
			{
				return FactionDiplomacyType.My_Faction;
			}

			if(GetFaction(factionA, out var _factionA) && GetFaction(factionB, out var _factionB))
			{
				return GetFactionDiplomacyType(_factionA, _factionB);
			}
			else
			{
				return FactionDiplomacyType.Neutral_Faction;
			}
		}
		public FactionDiplomacyType GetFactionDiplomacyType(FactionData factionA, FactionData factionB)
		{
			if(factionA == null || factionB == null) return FactionDiplomacyType.Neutral_Faction;
			if(factionA == factionB) return FactionDiplomacyType.Alliance_Faction;

			return GetFactionDiplomacyType(factionA.FactionIndex, factionB.FactionIndex);
		}
		public FactionDiplomacyType GetFactionDiplomacyType(DiplomacyItem factionA, DiplomacyItem factionB)
		{
			if(factionA == factionB)
			{
				return FactionDiplomacyType.My_Faction;
			}

			int A2B = factionA.GetFriendshipValue(factionB.FactionIndex);
			int B2A = factionB.GetFriendshipValue(factionA.FactionIndex);

			int min = Mathf.Min(A2B, B2A);
			int max = Mathf.Max(A2B, B2A);

			if(min + MaxDistance < max)
			{
				max = min + MaxDistance;
			}

			bool checkAlliance = min >= AllianceValue;
			bool checkPositive = min >= PositiveValue && max >= AllianceValue;
			bool checkNegative = max <= NegativeValue && min <= EnemyValue;
			bool checkEnemy = max <= EnemyValue;


			if(checkAlliance && checkPositive)
			{
				return FactionDiplomacyType.Alliance_Faction;
			}
			else if(!checkAlliance && checkPositive)
			{
				return FactionDiplomacyType.Positive_Faction;
			}
			else if(!checkEnemy && checkNegative)
			{
				return FactionDiplomacyType.Negative_Faction;
			}
			else if(checkEnemy && checkNegative)
			{
				return FactionDiplomacyType.Enemy_Faction;
			}
			else
			{
				return FactionDiplomacyType.Neutral_Faction;
			}
		}
	}
}
