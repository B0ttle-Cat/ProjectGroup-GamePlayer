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
	public interface ITacticalStateComponent : IPlayValue
	{
		public IOdccComponent UnitTacticalComponent { get; set; }
	}
	public interface IUnitStateValue : IPlayValue
		, IRetireStateValue, ITacticalCombatStateValue, ITacticalStateComponent
	{ }

	public class FireunitStateValue : DataObject, IUnitStateValue
	{
		private IFireunitData unitData;
		private bool isRetire;
		private ITacticalCombatStateValue.TacticalCombatStateType tacticalState;
		private IOdccComponent unitTacticalComponent;

		public IFireunitData UnitData { get => unitData; set => unitData=value; }
		public bool IsRetire { get => isRetire; set => isRetire=value; }
		public ITacticalCombatStateValue.TacticalCombatStateType TacticalCombatState { get => tacticalState; set => tacticalState=value; }
		public IOdccComponent UnitTacticalComponent { get => unitTacticalComponent; set => unitTacticalComponent=value; }


		protected override void Disposing()
		{
			unitData = null;
			unitTacticalComponent = null;
		}
	}
}
