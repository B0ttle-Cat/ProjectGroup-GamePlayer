using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="ITeamInteractiveValue"/>
	/// </summary>
	public class TeamInteractiveInfo
	{
		public ITeamInteractiveValue Actor { get; private set; }
		public ITeamInteractiveValue Target { get; private set; }
		public TeamInteractiveInfo(ITeamInteractiveValue actor, ITeamInteractiveValue target)
		{
			Actor=actor;
			Target=target;
		}
		public float Distance;          // 대상의 공간상 거리
		public Vector3 Direction;       // 대상의 공간상 방향

		//public bool IsInActionRange;
	}
}
