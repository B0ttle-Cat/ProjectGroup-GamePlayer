using System;

namespace BC.OdccBase
{
	[Serializable]
	public struct ProjectileHitReport
	{
		public (ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) Actor;

		public int[] HitTargetIndex;
		public HitType MissesHit;

		public ProjectileHitReport((ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) actor, HitType hitType, int[] hitTargetIndex)
		{
			Actor = actor;
			MissesHit = hitType;
			HitTargetIndex = hitTargetIndex;
		}

		public ProjectileHitReport((ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) actor, HitType misusingHitType)
		{
			Actor = actor;
			MissesHit = misusingHitType;
			HitTargetIndex = new int[0];
		}

		public enum HitType
		{
			None = 0,

			Hit = 10,
			Hit_일반_공격,


			Miss = 1000,
			Miss_투사체의_추적범위를_벗어남,
			Miss_타겟이_이미_무력화됨,
			Miss_타겟을_잃어버림,

			Miss_발주가_이미_무력화됨,
			Miss_발주를_일어버림,
		}
	}
}
