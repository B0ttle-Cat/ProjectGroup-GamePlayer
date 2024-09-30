using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamSpawnOnAnchorData : FireteamCommandData
	{
		[SerializeField]
		private IMapAnchor spawnAnchor;
		public IMapAnchor SpawnAnchor { get => spawnAnchor; set => spawnAnchor=value; }
	}
}
