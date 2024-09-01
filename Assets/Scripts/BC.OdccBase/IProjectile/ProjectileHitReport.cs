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
			Hit_�Ϲ�_����,


			Miss = 1000,
			Miss_����ü��_����������_���,
			Miss_Ÿ����_�̹�_����ȭ��,
			Miss_Ÿ����_�Ҿ����,

			Miss_���ְ�_�̹�_����ȭ��,
			Miss_���ָ�_�Ͼ����,
		}
	}
}
