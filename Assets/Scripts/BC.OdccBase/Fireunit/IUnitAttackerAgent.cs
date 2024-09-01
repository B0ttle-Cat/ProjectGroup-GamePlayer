using BC.ODCC;

namespace BC.OdccBase
{
	public interface IUnitAttackerAgent : IOdccComponent
	{
		public bool hasChange { get; set; }
		public IUnitInteractiveValue ThisActor { get; }
		public IUnitInteractiveValue ThisTarget { get; }
		public void InputAttackTarget(UnitInteractiveInfo info);
		public void InputAttackStop();
	}
}
