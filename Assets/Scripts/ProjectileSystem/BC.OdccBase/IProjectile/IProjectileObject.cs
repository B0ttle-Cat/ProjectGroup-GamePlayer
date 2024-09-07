using System;

using BC.ODCC;

namespace BC.OdccBase
{
	public interface IProjectileObject : IOdccObject
	{
		public (ICharacterAgent agent, IUnitInteractiveValue value) Actor { get; set; }
		public (ICharacterAgent agent, IUnitInteractiveValue value) Target { get; set; }
		public Action<IProjectileObject> OnHitCallback { get; set; }
		public IProjectileBallistics Ballistics { get; }
	}
}
