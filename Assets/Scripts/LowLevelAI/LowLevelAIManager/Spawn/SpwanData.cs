using BC.ODCC;

namespace BC.LowLevelAI
{
	public class SpawnData : DataObject
	{
		public MapAnchor spawnAnchorTarget;
		public int spawnUnitCount;
		public int spawnUnitIndex;
		public float spawnRadius = 2f;
		public float spawnRandomRadius = 1f;
		//	public int factionIndex;
		//	public int fireteamIndex;
		//	public int fireunitIndex;
	}
}
