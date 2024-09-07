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
			Hit_일반_공격,
			Hit_일반_스킬,

			Miss = 1000,
			Miss_투사체의_추적범위를_벗어남 = 1100,
			Miss_목표가_이미_무력화됨,
			Miss_목표를_잃어버림,

			Miss_사수가_이미_무력화됨 = 1200,
			Miss_사수를_일어버림,
		}
	}
}
