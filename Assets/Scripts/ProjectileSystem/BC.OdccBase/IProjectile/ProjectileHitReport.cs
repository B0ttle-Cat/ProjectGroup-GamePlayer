using System;

using UnityEngine;

namespace BC.OdccBase
{
	[Serializable]
	public struct ProjectileHitReport
	{
		public (ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) Actor;

		public Vector3Int[] hitTargetList;
		public ProjectileType projectileType;
		public SubProjectileType subProjectileType;
		public ProjectileHitReport((ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) actor, ProjectileType projectileType, SubProjectileType subProjectileType, Vector3Int[] hitTargetList)
		{
			Actor = actor;
			this.projectileType = projectileType;
			this.subProjectileType = subProjectileType;
			this.hitTargetList = hitTargetList;
		}

		public ProjectileHitReport((ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) actor, ProjectileType projectileType, SubProjectileType subProjectileType)
		{
			Actor = actor;
			this.projectileType = projectileType;
			this.subProjectileType = subProjectileType;
			hitTargetList = new Vector3Int[0];
		}

		public enum ProjectileType : int
		{
			Hit_�Ϲ�_����,

			Hit_�ַ�_��ų = 100,
			Hit_�Ϲ�_��ų,
			Hit_����_��ų,
			Hit_Ư��_��ų,
		}
		[Flags]
		public enum SubProjectileType : int
		{
			�̺з� = 0,

			���� = 1<<0,
			���� = 1<<1,
			���� = 1<<2,

			��ź = 1<<5,
			���� = 1<<6,
			������ = 1<<7,

			���� = 1<<15,
			���� = 1<<16,
			ȭ�� = 1<<17,
			���� = 1<<18,

			Ư��  = 1 << 31,

			// Set Preview
			����ź = ���� | ��ź | ����,
			����ź = ���� | ��ź | ���� | ����,

			����ź = ���� | ��ź | ����,
			�����ź = ���� | ��ź | ����,
			��ź = ���� | ��ź | ����,
			�ڰ���ź = ���� | ��ź | ����,

			������ź = ���� | ��ź | ����,
			ö����ź = ���� | ��ź | ����,
			Ȯ��ź = ���� | ��ź | ���� | ����,
			�̻��� = ���� | ���� | ����,

			���ϰ� = ���� | ������ | ����,
			����ź = ���� | ���� | ȭ�� | ����,
		}
	}
}
