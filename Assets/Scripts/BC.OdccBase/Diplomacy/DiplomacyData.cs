using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public class DiplomacyData : DataObject
	{
		[SerializeField]
#if UNITY_EDITOR
		[AssetSelector, ValidateInput("@IsMustNotNull(table)", "Is Must Not Null")]
#endif
		private DiplomacyTable table;
		public DiplomacyTable Table { get => table; set => table=value; }

		public bool GetDiplomacy(int index, out DiplomacyItem diplomacyItem)
		{
			int findIndex = Table.ItemList.FindIndex(item=>item.FactionIndex == index);
			if(findIndex >= 0)
			{
				diplomacyItem = Table.ItemList[findIndex];
				return true;
			}
			else
			{
				diplomacyItem = default;
				return false;
			}
		}
	}
}
