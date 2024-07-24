using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IUnitInteractiveValue"/>
	/// </summary>
	public class UnitInteractiveInfo : MemberInteractiveInfo
	{
		public IFireunitData Actor { get; private set; }
		public IFireunitData Target { get; private set; }
		public UnitInteractiveInfo(IFireunitData actor, IFireunitData target)
		{
			Actor=actor;
			Target=target;
		}

		public float Distance;          // ����� ������ �Ÿ�
		public Vector3 Direction;       // ����� ������ ����

		public bool IsInVisualRange;    // �þ� ���� �ȿ� ����
		public bool IsInActionRange;    // ���� ���� �ȿ� ����
		public bool IsInAttackRange;    // ���� ���� �ȿ� ����
	}
}
