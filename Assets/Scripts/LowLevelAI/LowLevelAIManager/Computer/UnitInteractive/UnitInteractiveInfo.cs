using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IUnitInteractiveValue"/>
	/// </summary>
	public class UnitInteractiveInfo
	{
		public IUnitInteractiveActor Actor { get; private set; }
		public IUnitInteractiveTarget Target { get; private set; }
		public UnitInteractiveInfo(IUnitInteractiveActor actor, IUnitInteractiveTarget target)
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
