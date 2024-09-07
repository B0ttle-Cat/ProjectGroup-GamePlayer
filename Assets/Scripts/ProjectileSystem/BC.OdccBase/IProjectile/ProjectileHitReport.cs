using System;

using UnityEngine;

namespace BC.OdccBase
{
	[Serializable]
	public struct ProjectileHitReport
	{
		public (ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) Actor;

		public Vector3Int[] HitTargetList;
		public HitType IsHitType;

		public bool IsHit => HitType.Hit <= IsHitType&& IsHitType < HitType.Miss;
		public bool IsMiss => HitType.Miss <= IsHitType;
		public ProjectileHitReport((ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) actor, HitType hitType, Vector3Int[] hitTargetIndex)
		{
			Actor = actor;
			IsHitType = hitType;
			HitTargetList = hitTargetIndex;
		}

		public ProjectileHitReport((ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) actor, HitType misusingHitType)
		{
			Actor = actor;
			IsHitType = misusingHitType;
			HitTargetList = new Vector3Int[0];
		}

		public enum HitType : int
		{
			None = 0,

			Hit = 10,
			Hit_�Ϲ�_����,
			Hit_�Ϲ�_��ų,

			Miss = 1000,
			Miss_����ü��_����������_��� = 1100,
			Miss_��ǥ��_�̹�_����ȭ��,
			Miss_��ǥ��_�Ҿ����,

			Miss_�����_�̹�_����ȭ�� = 1200,
			Miss_�����_�Ͼ����,
		}
	}
}
