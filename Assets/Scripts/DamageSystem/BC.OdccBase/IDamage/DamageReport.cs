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
			피해,
			회복,
			빗나감,
			차단됨,
		}
		[Flags]
		public enum SubDamageType
		{
			None = 0,
			최악 = 1<<0,
			무효 = 1<<1,
			저항 = 1<<2,
			일반 = 1<<3,
			증폭 = 1<<4,
			유효 = 1<<5,
			최고 = 1<<6,

			치명타 = 1<<10,
		}
	}

}
