using BC.ODCC;

namespace BC.LowLevelAI
{
	public class DiplomacyData : DataObject
	{
		[UnityEngine.SerializeField]
		private DiplomacyTable table;
		public DiplomacyTable Table { get => table; set => table=value; }
	}
}
