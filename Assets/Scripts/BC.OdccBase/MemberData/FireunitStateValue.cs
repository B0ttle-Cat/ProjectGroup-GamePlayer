using BC.ODCC;

namespace BC.OdccBase
{
	public interface IUnitStateValue : IOdccData
	{
		public bool IsRetire { get; set; }
		public TacticalCombatStateType TacticalCombatState { get; set; }
		public enum TacticalCombatStateType
		{
			Attack,     // Ÿ���� ����
			Move,       // Ÿ�� ������ ���� �̵�
			None,       // Ÿ�� ����
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
