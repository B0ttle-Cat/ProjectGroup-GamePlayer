using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class TeleportationData : FireunitCommandData
	{
		public IMapAnchor targetAnchor;
		public int totalUnitCount;
		public int unitIndex;
		public float targetRadius = 2f;
		public float targetRandomRadius = 1f;
	}
}