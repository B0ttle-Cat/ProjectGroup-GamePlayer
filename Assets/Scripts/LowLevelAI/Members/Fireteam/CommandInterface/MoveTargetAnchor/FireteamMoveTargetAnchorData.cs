using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMoveTargetAnchorData : FireteamCommandData
	{
		[SerializeField]
		private IMapPathNode moveTargetPathNode;
		public IMapPathNode MovePathNode { get => moveTargetPathNode; set => moveTargetPathNode=value; }
		public bool HasMoveTarget { get => MovePathNode != null && MovePathNode.ThisPoint != null; }
	}
}
