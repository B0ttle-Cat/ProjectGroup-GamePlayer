using BC.ODCC;

namespace BC.OdccBase
{
	public interface IRetireStateValue : IPlayValue
	{
		public bool IsRetire { get; set; }
	}

	public interface ITacticalCombatStateValue : IPlayValue
	{
		public TacticalCombatStateType TacticalCombatState { get; set; }
		public enum TacticalCombatStateType
		{
			Attack,     // 타겟을 공격
			Move,       // 타겟 공격을 위해 이동
			None,       // 타겟 없음
		}
	}
	public interface ITacticalCombatStateComponent : IPlayValue
	{
		public IOdccComponent UnitTacticalCombatStateUpdate { get; set; }
		public IOdccComponent UnitTacticalCombatStateChanger { get; set; }
	}
	public interface IUnitStateValue : IPlayValue
		, IRetireStateValue, ITacticalCombatStateValue, ITacticalCombatStateComponent
	{ }

	public class FireunitStateValue : DataObject, IUnitStateValue
	{
		private IFireunitData unitData;
		private bool isRetire;
		private ITacticalCombatStateValue.TacticalCombatStateType tacticalCombatState;
		private IOdccComponent unitTacticalCombatStateUpdate;
		private IOdccComponent unitTacticalCombatStateChanger;

		public IFireunitData UnitData { get => unitData; set => unitData=value; }
		public bool IsRetire { get => isRetire; set => isRetire=value; }
		public ITacticalCombatStateValue.TacticalCombatStateType TacticalCombatState { get => tacticalCombatState; set => tacticalCombatState=value; }
		public IOdccComponent UnitTacticalCombatStateUpdate { get => unitTacticalCombatStateUpdate; set => unitTacticalCombatStateUpdate=value; }
		public IOdccComponent UnitTacticalCombatStateChanger { get => unitTacticalCombatStateChanger; set => unitTacticalCombatStateChanger=value; }


		protected override void Disposing()
		{
			unitData = null;
			tacticalCombatState = ITacticalCombatStateValue.TacticalCombatStateType.None;
			unitTacticalCombatStateUpdate = null;
			unitTacticalCombatStateChanger = null;
		}

	}
}
