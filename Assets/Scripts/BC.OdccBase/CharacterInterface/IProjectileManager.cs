using UnityEngine;

namespace BC.Base
{
	public interface IShotterProjectile
	{

	}
	public interface ITargetProjectile
	{

	}
	public interface IBulletProjectile
	{
		public bool CreateBullet(IShotterProjectile iShotter, GameObject bulletObject) { return false; }
	}
}
