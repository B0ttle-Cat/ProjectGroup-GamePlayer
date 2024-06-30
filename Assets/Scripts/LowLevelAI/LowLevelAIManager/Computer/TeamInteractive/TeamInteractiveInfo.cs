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
		public float Distance;          // ����� ������ �Ÿ�
		public Vector3 Direction;       // ����� ������ ����
	}
}
