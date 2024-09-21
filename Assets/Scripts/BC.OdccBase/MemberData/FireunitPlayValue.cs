using BC.ODCC;

using static BC.OdccBase.AbilityMath;

namespace BC.OdccBase
{
	public interface IUnitPlayValue : IOdccData
	{
		public AbilityValue<float> VisualRange { get; set; }
		public AbilityRange<float> ActionRange { get; set; }
		public AbilityValue<float> AttackRange { get; set; }
		public AbilityValue<int> HealthPoint { get; set; }
		public AbilityValue<float> OffenseDamagePoint { get; set; }
		public AbilityValue<float> OffenseRandomRate { get; set; }

		public AbilityValue<float> CriticalProbabilityPoint { get; set; }
		public AbilityValue<float> CriticalAttackMultiplier { get; set; }
		public AbilityValue<float> Anti_CriticalProbabilityPoint { get; set; }
		public AbilityValue<float> Anti_CriticalAttackMultiplier { get; set; }

		public AbilityValue<float> DefenseDamagePoint { get; set; }
		public AbilityValue<float> DefenseRandomRate { get; set; }

		public AbilityValue<float> AccuracyPoint { get; set; }
		public AbilityValue<float> Anti_AccuracyPoint { get; set; }
		public AbilityValue<float> RecoveryPoint { get; set; }
	}

	public class FireunitPlayValue : DataObject, IUnitPlayValue
	{
		protected override void Disposing()
		{
		}

		#region Range
		private AbilityValue<float> visualRange;
		private AbilityRange<float> actionRange;
		private AbilityValue<float> attackRange;
		#endregion
		#region Point
		private AbilityValue<int> healthPoint;
		private AbilityValue<float> offenseDamagePoint;
		private AbilityValue<float> offenseRandomRate;
		private AbilityValue<float> defenseDamagePoint;
		private AbilityValue<float> defenseRandomRate;
		private AbilityValue<float> accuracyPoint;
		private AbilityValue<float> anti_accuracyPoint;
		private AbilityValue<float> recoveryPoint;

		private AbilityValue<float> criticalPoint;
		private AbilityValue<float> criticalAttackMultiplier;
		private AbilityValue<float> anti_criticalProbabilityPoint;
		private AbilityValue<float> anti_criticalAttackMultiplier;

		#region ConstValue
		public const float MinVisualRange = 2f;
		public const float MaxVisualRange = 10f;
		public const float MinActionRange = 10f;
		public const float MaxActionRange = 15f;
		public const float MinAttackRange = 2f;
		public const float MaxAttackRange = 20f;
		#endregion

		#endregion

		#region Range
		public AbilityValue<float> VisualRange { get => visualRange; set => visualRange = value; }
		public AbilityRange<float> ActionRange { get => actionRange; set => actionRange = value; }
		public AbilityValue<float> AttackRange { get => attackRange; set => attackRange=value; }
		#endregion
		#region Point
		public AbilityValue<int> HealthPoint { get => healthPoint; set => healthPoint = value; }
		public AbilityValue<float> OffenseDamagePoint { get => offenseDamagePoint; set => offenseDamagePoint = value; }
		public AbilityValue<float> OffenseRandomRate { get => offenseRandomRate; set => offenseRandomRate=value; }
		public AbilityValue<float> DefenseDamagePoint { get => defenseDamagePoint; set => defenseDamagePoint = value; }
		public AbilityValue<float> DefenseRandomRate { get => defenseRandomRate; set => defenseRandomRate=value; }
		public AbilityValue<float> AccuracyPoint { get => accuracyPoint; set => accuracyPoint = value; }
		public AbilityValue<float> Anti_AccuracyPoint { get => anti_accuracyPoint; set => anti_accuracyPoint = value; }
		public AbilityValue<float> RecoveryPoint { get => recoveryPoint; set => recoveryPoint = value; }
		public AbilityValue<float> CriticalProbabilityPoint { get => criticalPoint; set => criticalPoint=value; }
		public AbilityValue<float> CriticalAttackMultiplier { get => criticalAttackMultiplier; set => criticalAttackMultiplier=value; }
		public AbilityValue<float> Anti_CriticalProbabilityPoint { get => anti_criticalProbabilityPoint; set => anti_criticalProbabilityPoint=value; }
		public AbilityValue<float> Anti_CriticalAttackMultiplier { get => anti_criticalAttackMultiplier; set => anti_criticalAttackMultiplier=value; }
		#endregion

	}
}
