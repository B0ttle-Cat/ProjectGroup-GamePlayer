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

		public float Distance;          // 대상의 공간상 거리
		public Vector3 Direction;       // 대상의 공간상 방향

		public bool IsInVisualRange;    // 시야 범위 안에 있음
		public bool IsInActionRange;    // 대응 범위 안에 있음
		public bool IsInAttackRange;    // 공격 범위 안에 있음
	}
}
