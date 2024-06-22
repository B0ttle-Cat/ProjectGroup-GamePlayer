using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMoveTargetAnchorData : FireteamCommandData
	{
		[SerializeField]
		private MapPathNode moveTargetPathNode;
		public MapPathNode MovePathNode { get => moveTargetPathNode; set => moveTargetPathNode=value; }
		public bool HasMoveTarget { get => MovePathNode != null && MovePathNode.ThisPoint != null; }
	}
}
