using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public enum FactionDiplomacyType : int
	{
		[InspectorName("Enemy")]
		Enemy_Faction = -10,        // ������ ����
		[InspectorName("Neutral")]
		Neutral_Faction = 0,        // �߸��� ����
		[InspectorName("Alliance")]
		Alliance_Faction = 10,      // ���� ����
		[InspectorName("Equal")]
		Equal_Faction = 100,        // ���� ����
	}
	public interface IFactionDiplomacyData : IOdccData
	{
		public FactionDiplomacyType GetFactionDiplomacyType(IFactionData factionData);
	}
	public class FactionDiplomacyData : DataObject, IFactionDiplomacyData
	{
		public int thisFactionData;
		public List<int> enemyFactionList;
		public List<int> allianceFactionList;
		public List<int> neutralFactionList;

		public FactionDiplomacyType GetFactionDiplomacyType(IFactionData factionData)
		{
			if(factionData.IsEqualsFaction(thisFactionData))
			{
				return FactionDiplomacyType.Equal_Faction;
			}
			int listCount = enemyFactionList.Count;
			for(int i = 0 ; i < listCount ; i++)
			{
				if(factionData.IsEqualsFaction(enemyFactionList[i]))
				{
					return FactionDiplomacyType.Enemy_Faction;
				}
			}
			listCount = allianceFactionList.Count;
			for(int i = 0 ; i < listCount ; i++)
			{
				if(factionData.IsEqualsFaction(enemyFactionList[i]))
				{
					return FactionDiplomacyType.Alliance_Faction;
				}
			}
			return FactionDiplomacyType.Neutral_Faction;
		}
	}
}
