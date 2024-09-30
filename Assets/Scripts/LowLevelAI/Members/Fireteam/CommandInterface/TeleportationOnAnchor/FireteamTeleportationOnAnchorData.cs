using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamTeleportationOnAnchorData : FireteamCommandData
	{
		[SerializeField]
		private IMapAnchor teleportationAnchor;
		public IMapAnchor TeleportationAnchor { get => teleportationAnchor; set => teleportationAnchor=value; }
	}
}
