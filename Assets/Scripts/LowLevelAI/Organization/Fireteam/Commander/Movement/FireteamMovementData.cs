using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMovementData : FireteamCommandData
	{
		[SerializeField]
		private FireteamMembers fireteamMembers;
		public FireteamMembers Members { get => fireteamMembers; set => fireteamMembers = value; }

		[SerializeField]
		private MapPathNode moveTargetPathNode;
		public MapPathNode MovePathNode { get => moveTargetPathNode; set => moveTargetPathNode=value; }
		public bool HasMoveTarget { get => MovePathNode != null && MovePathNode.ThisPoint != null; }
	}
}
