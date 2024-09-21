using BC.ODCC;

namespace BC.OdccBase
{
	public interface IUnitStateValue : IOdccData
	{
		public bool IsRetire { get; set; }
		public TacticalCombatStateType TacticalCombatState { get; set; }
		public enum TacticalCombatStateType
		{
			Attack,     // 타겟을 공격
			Move,       // 타겟 공격을 위해 이동
			None,       // 타겟 없음
		}
		public IOdccComponent UnitTacticalCombatStateUpdate { get; set; }
		public IOdccComponent UnitTacticalCombatStateChanger { get; set; }
	}

	public class FireunitStateValue : DataObject, IUnitStateValue
	{
		private bool isRetire;
		private IUnitStateValue.TacticalCombatStateType tacticalCombatState;
		private IOdccComponent unitTacticalCombatStateUpdate;
		private IOdccComponent unitTacticalCombatStateChanger;

		public bool IsRetire { get => isRetire; set => isRetire=value; }
		public IUnitStateValue.TacticalCombatStateType TacticalCombatState { get => tacticalCombatState; set => tacticalCombatState=value; }
		public IOdccComponent UnitTacticalCombatStateUpdate { get => unitTacticalCombatStateUpdate; set => unitTacticalCombatStateUpdate=value; }
		public IOdccComponent UnitTacticalCombatStateChanger { get => unitTacticalCombatStateChanger; set => unitTacticalCombatStateChanger=value; }


		protected override void Disposing()
		{
			tacticalCombatState = IUnitStateValue.TacticalCombatStateType.None;
			unitTacticalCombatStateUpdate = null;
			unitTacticalCombatStateChanger = null;
		}
	}
}
