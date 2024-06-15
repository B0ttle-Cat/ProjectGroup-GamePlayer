using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamStateData : OdccStateData,
		IStayStateData//,
					  //IMovementStateData
	{
		[SerializeField]
		private MapPathNode moveTargetPathNode;

		public MapPathNode MovePathNode { get => moveTargetPathNode; set => moveTargetPathNode=value; }
		public bool HasMoveTarget { get => MovePathNode != null && MovePathNode.ThisPoint != null; }
		public bool IsStay { get; set; }
		public bool IsMovement { get; set; }

		protected override void Disposing()
		{
			moveTargetPathNode = null;
		}

	}
}
