using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamSpawnOnAnchorData : FireteamCommandData
	{
		[SerializeField]
		private MapAnchor spawnAnchor;
		public MapAnchor SpawnAnchor { get => spawnAnchor; set => spawnAnchor=value; }
	}
}
