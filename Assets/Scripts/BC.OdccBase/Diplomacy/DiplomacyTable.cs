using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	[CreateAssetMenu(fileName = "new Table", menuName = "BC/Faction/new DiplomacyTableList")]
	public class DiplomacyTable : SerializedScriptableObject
	{
		[TableList]
		public List<DiplomacyItem> ItemList;

		public int AllianceValue = 50;
		public int PositiveValue = 25;
		public int NegativeValue = -25;
		public int EnemyValue = -50;
		public int MaxDistance = 50;

#if UNITY_EDITOR
		private void OnValidate()
		{
			SelectDiplomacyTable = this;

			ItemList ??= new List<DiplomacyItem>();


			for(int i = 0 ; i < ItemList.Count ; i++)
			{
				DiplomacyItem diplomacyItem = ItemList[i];
				diplomacyItem.FactionIndex = i;
				ItemList[i] = diplomacyItem;
			}
		}
		public static DiplomacyTable SelectDiplomacyTable = null;
#endif
	}

}
