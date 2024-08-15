using BC.ODCC;

namespace BC.OdccBase
{
	public interface IRetireStateValue : IPlayValue
	{
		public bool IsRetire { get; set; }
	}

	public interface ITacticalStateValue : IPlayValue
	{
		public TacticalStateType TacticalState { get; set; }
		public enum TacticalStateType
		{
			None = 0,
			Attack,
			MovePos,
			HoldPos
		}
	}
	public interface ITacticalStateComponent : IPlayValue
	{
		public IOdccComponent UnitTacticalComponent { get; set; }
	}
	public interface IUnitStateValue : IPlayValue
		, IRetireStateValue, ITacticalStateValue, ITacticalStateComponent
	{ }

	public class FireunitStateValue : DataObject, IUnitStateValue
	{
		private IFireunitData unitData;
		private bool isRetire;
		private ITacticalStateValue.TacticalStateType tacticalState;
		private IOdccComponent unitTacticalComponent;

		public IFireunitData UnitData { get => unitData; set => unitData=value; }
		public bool IsRetire { get => isRetire; set => isRetire=value; }
		public ITacticalStateValue.TacticalStateType TacticalState { get => tacticalState; set => tacticalState=value; }
		public IOdccComponent UnitTacticalComponent { get => unitTacticalComponent; set => unitTacticalComponent=value; }
	}
}
