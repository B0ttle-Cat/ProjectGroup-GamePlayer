using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamTeleportationOnAnchorData : FireteamCommandData
	{
		[SerializeField]
		private MapAnchor teleportationAnchor;
		public MapAnchor TeleportationAnchor { get => teleportationAnchor; set => teleportationAnchor=value; }
	}
}
