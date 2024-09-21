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
			Hit_ÀÏ¹Ý_°ø°Ý,

			Hit_ÁÖ·Â_½ºÅ³ = 100,
			Hit_ÀÏ¹Ý_½ºÅ³,
			Hit_º¸Á¶_½ºÅ³,
			Hit_Æ¯¼ö_½ºÅ³,
		}
		[Flags]
		public enum SubProjectileType : int
		{
			¹ÌºÐ·ù = 0,

			¼ÒÇü = 1<<0,
			ÁßÇü = 1<<1,
			´ëÇü = 1<<2,

			ÃÑÅº = 1<<5,
			·ÎÄÏ = 1<<6,
			¿¡³ÊÁö = 1<<7,

			Æø¹ß = 1<<15,
			°üÅë = 1<<16,
			È­ÇÐ = 1<<17,
			´ÙÁß = 1<<18,

			Æ¯¼ö  = 1 << 31,

			// Set Preview
			±ÇÃÑÅº = ¼ÒÇü | ÃÑÅº | °üÅë,
			¼¦°ÇÅº = ¼ÒÇü | ÃÑÅº | °üÅë | ´ÙÁß,

			¼ÒÃÑÅº = ÁßÇü | ÃÑÅº | °üÅë,
			±â°üÃÑÅº = ÁßÇü | ÃÑÅº | °üÅë,
			À¯Åº = ÁßÇü | ÃÑÅº | Æø¹ß,
			¹Ú°ÝÆ÷Åº = ÁßÇü | ÃÑÅº | Æø¹ß,

			¼ºÇüÆ÷Åº = ´ëÇü | ÃÑÅº | Æø¹ß,
			Ã¶°©Æ÷Åº = ´ëÇü | ÃÑÅº | °üÅë,
			È®»êÅº = ´ëÇü | ÃÑÅº | °üÅë | ´ÙÁß,
			¹Ì»çÀÏ = ´ëÇü | ·ÎÄÏ | Æø¹ß,

			·¹ÀÏ°Ç = ÁßÇü | ¿¡³ÊÁö | °üÅë,
			¼ÒÀÌÅº = ÁßÇü | ·ÎÄÏ | È­ÇÐ | ´ÙÁß,
		}
	}
}
