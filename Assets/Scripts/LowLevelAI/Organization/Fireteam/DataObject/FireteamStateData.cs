using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamStateData : OdccStateData,
		IStayStateData,
		IMovementStateData
	{
		[SerializeField]
		private MapPathPoint moveTargetPoint;


		public MapPathPoint MoveTargetPoint { get => moveTargetPoint; set => moveTargetPoint=value; }
		public bool HasMoveTarget { get => MoveTargetPoint != null; }
		public bool IsStay { get; set; }
		public bool IsMovement { get; set; }
	}
}
