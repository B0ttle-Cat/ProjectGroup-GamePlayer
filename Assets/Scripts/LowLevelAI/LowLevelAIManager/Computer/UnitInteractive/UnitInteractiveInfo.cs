using BC.OdccBase;

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
		public UnitInteractiveInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, FactionDiplomacyType diplomacyType)
		{
			Actor=actor;
			Target=target;
			this.DiplomacyType = diplomacyType;
		}

		// Compute
		public FactionDiplomacyType DiplomacyType; // 대상의 외교 관계

		public float Distance;          // 대상의 공간상 거리
		public Vector3 Direction;       // 대상의 공간상 방향

		public bool IsEqualFaction;     // 시야 범위 안에 있음
		public bool IsEqualTeam;        // 시야 범위 안에 있음

		// Flag
		public bool IsInVisualRange;    // 시야 범위 안에 있음
		public bool IsInActionRange;    // 대응 범위 안에 있음
		public bool IsInAttackRange;    // 공격 범위 안에 있음
	}
}
