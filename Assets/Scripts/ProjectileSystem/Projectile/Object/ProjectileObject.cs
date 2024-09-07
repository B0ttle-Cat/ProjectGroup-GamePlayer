using System;

using BC.ODCC;
using BC.OdccBase;

namespace BC.ProjectileSystem
{
	public class ProjectileObject : ObjectBehaviour, IProjectileObject
	{
		public (ICharacterAgent agent, IUnitInteractiveValue value) Actor { get; set; }
		public (ICharacterAgent agent, IUnitInteractiveValue value) Target { get; set; }
		public Action<IProjectileObject> OnHitCallback { get; set; }
		public IProjectileBallistics Ballistics { get; private set; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			Ballistics = ThisContainer.GetComponent<IProjectileBallistics>();
		}
	}
}
