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
		public AbilityValue VisualRange { get; set; }
	}
	public interface IActionRangePlayValue : IOdccData
	{
		public AbilityRange ActionRange { get; set; }
	}
	public interface IAttackRangePlayValue : IOdccData
	{
		public AbilityValue AttackRange { get; set; }
	}

	public interface IHealthPlayValue : IOdccData
	{
		public AbilityValue HealthPoint { get; set; }
	}
	public interface IAttackPlayValue : IOdccData
	{
		public AbilityValue AttackPoint { get; set; }
	}
	public interface IDefensePlayValue : IOdccData
	{
		public AbilityValue DefensePoint { get; set; }
	}
	public interface IAccuracyPlayValue : IOdccData
	{
		public AbilityValue AccuracyPoint { get; set; }
	}
	public interface IEvasionPlayValue : IOdccData
	{
		public AbilityValue EvasionPoint { get; set; }
	}
	public interface IRecoveryPlayValue : IOdccData
	{
		public AbilityValue RecoveryPoint { get; set; }
	}

	public interface IUnitPlayValue : IPlayValue
		, IVisualRangePlayValue, IActionRangePlayValue, IAttackRangePlayValue
		, IHealthPlayValue, IAttackPlayValue, IDefensePlayValue, IAccuracyPlayValue, IEvasionPlayValue, IRecoveryPlayValue
	{ }

	public class FireunitPlayValue : DataObject, IUnitPlayValue
	{
		#region UnitData
		private IFireunitData unitData;
		#endregion
		#region Range
		private AbilityValue visualRange;
		private AbilityRange actionRange;
		private AbilityValue attackRange;
		#endregion
		#region Point
		private AbilityValue healthPoint;
		private AbilityValue attackPoint;
		private AbilityValue defensePoint;
		private AbilityValue accuracyPoint;
		private AbilityValue evasionPoint;
		private AbilityValue recoveryPoint;
		#endregion
		#region UnitData
		public IFireunitData UnitData { get => unitData; set => unitData = value; }
		#endregion
		#region Range
		public AbilityValue VisualRange { get => visualRange; set => visualRange = value; }
		public AbilityRange ActionRange { get => actionRange; set => actionRange = value; }
		public AbilityValue AttackRange { get => attackRange; set => attackRange=value; }
		#endregion
		#region Point
		public AbilityValue HealthPoint { get => healthPoint; set => healthPoint = value; }
		public AbilityValue AttackPoint { get => attackPoint; set => attackPoint = value; }
		public AbilityValue DefensePoint { get => defensePoint; set => defensePoint = value; }
		public AbilityValue AccuracyPoint { get => accuracyPoint; set => accuracyPoint = value; }
		public AbilityValue EvasionPoint { get => evasionPoint; set => evasionPoint = value; }
		public AbilityValue RecoveryPoint { get => recoveryPoint; set => recoveryPoint = value; }
		#endregion


		protected override void Disposing()
		{
			unitData = null;
		}
	}
}
