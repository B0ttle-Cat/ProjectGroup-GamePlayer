using BC.ODCC;

namespace BC.OdccBase
{
	public class DiplomacyData : DataObject
	{
		[UnityEngine.SerializeField]
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
