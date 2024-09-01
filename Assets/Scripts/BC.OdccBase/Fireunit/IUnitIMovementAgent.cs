using BC.ODCC;

using UnityEngine;
using UnityEngine.AI;

namespace BC.OdccBase
{
	public interface IUnitIMovementAgent : IOdccComponent
	{
		public NavMeshAgent NavMeshAgent { get; }
		public void InputMoveTarget(Vector3 target);
		public void InputMoveTarget(Vector3 target, bool isWarp);
		public void InputMoveStop(Vector3? stopWarp = null);
	}

}
