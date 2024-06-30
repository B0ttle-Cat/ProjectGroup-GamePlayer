using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="ITeamInteractiveValue"/>
	/// </summary>
	public class TeamInteractiveInfo
	{
		public ITeamInteractiveActor Actor { get; private set; }
		public ITeamInteractiveTarget Target { get; private set; }
		public TeamInteractiveInfo(ITeamInteractiveActor actor, ITeamInteractiveTarget target)
		{
			Actor=actor;
			Target=target;
		}
		public float Distance;          // 대상의 공간상 거리
		public Vector3 Direction;       // 대상의 공간상 방향
	}
}
