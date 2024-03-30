using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	[CreateAssetMenu(fileName = "new Table", menuName = "BC/Faction/new DiplomacyTableList")]
	public class DiplomacyTable : SerializedScriptableObject
	{
		[TableList]
		public List<DiplomacyItem> ItemList;
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
