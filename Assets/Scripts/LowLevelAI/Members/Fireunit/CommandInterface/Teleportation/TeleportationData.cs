namespace BC.LowLevelAI
{
	public class TeleportationData : FireunitCommandData
	{
		public MapAnchor targetAnchor;
		public int totalUnitCount;
		public int unitIndex;
		public float targetRadius = 2f;
		public float targetRandomRadius = 1f;
	}
}