using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IUnitInteractiveValue"/>
	/// </summary>
	public class UnitInteractiveInfo
	{
		public IUnitInteractiveValue Actor { get; private set; }
		public IUnitInteractiveValue Target { get; private set; }
		public UnitInteractiveInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, FactionDiplomacyType diplomacyType)
		{
			Actor=actor;
			Target=target;
			this.DiplomacyType = diplomacyType;

			IsEqualFaction = Actor.UnitData.IsEqualsFaction(Target.UnitData);
			IsEqualFaction = Target.UnitData.IsEqualsFaction(Target.UnitData);
		}

		// Compute : Actor �������� ������
		public FactionDiplomacyType DiplomacyType; // ����� �ܱ� ����

		public float Distance;          // ����� ������ �Ÿ�
		public Vector3 Direction;       // ����� ������ ����

		public bool IsEqualFaction;     // ���� ���� Ȯ��
		public bool IsEqualTeam;        // ���� �� Ȯ��

		public bool IsInActionRange;
		public bool IsInActionStartRange;   // ���� ���� ���� �ȿ� ���� : ���� ���� == Unit AI�� Ÿ�ٿ� ���� �ϴ� �Ÿ�
		public bool IsInActionEndedRange;   // ���� �ߴ� ���� �ȿ� ���� : ���� ���� == Unit AI�� Ÿ�ٿ� ���� �ϴ� �Ÿ�

		public bool IsInAttackRange;
	}
}
