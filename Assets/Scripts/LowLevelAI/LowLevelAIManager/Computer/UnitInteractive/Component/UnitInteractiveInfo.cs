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

		// Compute : Actor 입장으로 서술함
		public FactionDiplomacyType DiplomacyType; // 대상의 외교 관계

		public float Distance;          // 대상의 공간상 거리
		public Vector3 Direction;       // 대상의 공간상 방향

		public bool IsEqualFaction;     // 동일 새력 확인
		public bool IsEqualTeam;        // 동일 팀 확인

		public bool IsInActionRange;
		public bool IsInActionStartRange;   // 대응 시작 범위 안에 있음 : 대응 범위 == Unit AI가 타겟에 대응 하는 거리
		public bool IsInActionEndedRange;   // 대응 중단 범위 안에 있음 : 대응 범위 == Unit AI가 타겟에 대응 하는 거리

		public bool IsInAttackRange;
	}
}
