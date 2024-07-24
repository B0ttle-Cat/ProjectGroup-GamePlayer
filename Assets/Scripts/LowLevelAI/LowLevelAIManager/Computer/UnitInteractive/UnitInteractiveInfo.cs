using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IUnitInteractiveValue"/>
	/// </summary>
	public class UnitInteractiveInfo : MemberInteractiveInfo
	{
		public IUnitInteractiveValue Actor { get; private set; }
		public IUnitInteractiveValue Target { get; private set; }
		public UnitInteractiveInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target)
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
