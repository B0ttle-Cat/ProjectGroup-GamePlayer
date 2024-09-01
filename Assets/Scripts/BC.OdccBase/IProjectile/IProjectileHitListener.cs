using BC.ODCC;

namespace BC.OdccBase
{
	public interface IProjectileHitListener : IOdccObject
	{
		public void OnHit(IProjectileObject projectileObject, ProjectileHitReport hitReport);
	}
}
