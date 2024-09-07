using BC.ODCC;

namespace BC.OdccBase
{
	public interface IProjectileHitListener : IOdccObject
	{
		public void OnHit(IProjectileObject projectileObject, in ProjectileHitReport hitReport);
	}
}
