using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IProjectileSystem : IOdccObject
	{
		void OnCreateProjectile(ObjectBehaviour projectilePrefab, Pose onFirePose,
			(ICharacterAgent agent, IUnitInteractiveValue value) actor,
			(ICharacterAgent agent, IUnitInteractiveValue value) target);
	}
}
