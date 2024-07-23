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
		public float Distance;          // ����� ������ �Ÿ�
		public Vector3 Direction;       // ����� ������ ����

		//public bool IsInActionRange;
	}
}
