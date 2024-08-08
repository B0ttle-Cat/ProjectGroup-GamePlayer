using BC.ODCC;

using static BC.OdccBase.AbilityMath;

namespace BC.OdccBase
{
	public interface IPlayValue : IOdccData
	{
		public IFireunitData UnitData { get; set; }
	}

	public interface IVisualRangePlayValue : IOdccData
	{
		public ValueMinMax VisualRange { get; set; }
	}
	public interface IActionRangePlayValue : IOdccData
	{
		public ValueRange ActionRange { get; set; }
	}

	public interface IHealthPlayValue : IOdccData
	{
		public ValueMinMax HealthPoint { get; set; }
	}
	public interface IAttackPlayValue : IOdccData
	{
		public ValueMinMax AttackPoint { get; set; }
	}
	public interface IDefensePlayValue : IOdccData
	{
		public ValueMinMax DefensePoint { get; set; }
	}
	public interface IAccuracyPlayValue : IOdccData
	{
		public ValueMinMax AccuracyPoint { get; set; }
	}
	public interface IEvasionPlayValue : IOdccData
	{
		public ValueMinMax EvasionPoint { get; set; }
	}
	public interface IRecoveryPlayValue : IOdccData
	{
		public ValueMinMax RecoveryPoint { get; set; }
	}

	public interface IUnitPlayValue : IPlayValue
		, IVisualRangePlayValue, IActionRangePlayValue
		, IHealthPlayValue, IAttackPlayValue, IDefensePlayValue, IAccuracyPlayValue, IEvasionPlayValue, IRecoveryPlayValue
	{ }

	public class FireunitPlayValue : DataObject, IUnitPlayValue
	{
		private IFireunitData unitData;
		private ValueMinMax visualRange;
		private ValueRange actionRange;
		private ValueMinMax healthPoint;
		private ValueMinMax attackPoint;
		private ValueMinMax defensePoint;
		private ValueMinMax accuracyPoint;
		private ValueMinMax evasionPoint;
		private ValueMinMax recoveryPoint;

		public IFireunitData UnitData { get => unitData; set => unitData = value; }
		public ValueMinMax VisualRange { get => visualRange; set => visualRange = value; }
		public ValueRange ActionRange { get => actionRange; set => actionRange = value; }
		public ValueMinMax HealthPoint { get => healthPoint; set => healthPoint = value; }
		public ValueMinMax AttackPoint { get => attackPoint; set => attackPoint = value; }
		public ValueMinMax DefensePoint { get => defensePoint; set => defensePoint = value; }
		public ValueMinMax AccuracyPoint { get => accuracyPoint; set => accuracyPoint = value; }
		public ValueMinMax EvasionPoint { get => evasionPoint; set => evasionPoint = value; }
		public ValueMinMax RecoveryPoint { get => recoveryPoint; set => recoveryPoint = value; }
	}
}
