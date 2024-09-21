using System;

using UnityEngine;

namespace BC.OdccBase
{
	public struct DamageReport
	{
		public Vector3Int Actor { get; private set; }
		public Vector3Int Target { get; private set; }

		public int value;
		public DamageType damageType;
		public SubDamageType subDamageType;


		public DamageReport(IUnitInteractiveValue actor, IUnitInteractiveValue target)
		{
			Actor = actor.MemberUniqueID;
			Target = target.MemberUniqueID;
			this.value = 0;
			this.damageType = DamageType.None;
			this.subDamageType = SubDamageType.None;
		}
		public void SetReport(int value, DamageType damageType)
		{
			this.value = value;
			this.damageType=damageType;
		}

		public void SetReport(int value, DamageType damageType, SubDamageType subDamageType)
		{
			this.value = value;
			this.damageType=damageType;
			this.subDamageType=subDamageType;
		}

		public enum DamageType
		{
			None = 0,
			����,
			ȸ��,
			������,
			���ܵ�,
		}
		[Flags]
		public enum SubDamageType
		{
			None = 0,
			�־� = 1<<0,
			��ȿ = 1<<1,
			���� = 1<<2,
			�Ϲ� = 1<<3,
			���� = 1<<4,
			��ȿ = 1<<5,
			�ְ� = 1<<6,

			ġ��Ÿ = 1<<10,
		}
	}

}
