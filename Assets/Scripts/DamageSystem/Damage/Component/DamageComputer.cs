using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

using static BC.OdccBase.DamageReport;
using static BC.OdccBase.IUnitTypeValue;

namespace BC.DamageSystem
{
	public class DamageComputer : ComponentBehaviour
	{
		FieldAdvantageLevel.FieldType fieldType;

		public struct AttackAffinityAdvantage
		{
			public AdvantageLevel level;
			public float affinityAttackDamage;

			public SubDamageType SubDamageType => level switch {
				AdvantageLevel.�־� => SubDamageType.�־�,
				AdvantageLevel.��ȿ => SubDamageType.��ȿ,
				AdvantageLevel.���� => SubDamageType.����,
				AdvantageLevel.�Ϲ� => SubDamageType.�Ϲ�,
				AdvantageLevel.���� => SubDamageType.����,
				AdvantageLevel.��ȿ => SubDamageType.��ȿ,
				AdvantageLevel.�ְ� => SubDamageType.�ְ�,
				_ => SubDamageType.�Ϲ�,
			};
		}
		public struct FieldAdvantage
		{
			public AdvantageLevel level;
			public float attackDamage;
			public float accuracyProbability;
			public float criticalProbability;

			public float anti_attackDamage;
			public float anti_accuracyProbability;
			public float anti_criticalProbability;
		}

		internal void OnDamageCompute(IUnitInteractiveValue actor, IUnitInteractiveValue[] targets, ProjectileHitReport.ProjectileType projectileType)
		{
			int length = targets.Length;
			for(int i = 0 ; i < length ; i++)
			{
				IUnitInteractiveValue target = targets[i];
				DamageCompute(actor, target, i, length);
			}
		}
		protected virtual async void DamageCompute(IUnitInteractiveValue actor, IUnitInteractiveValue target, int targetIndex, int targetCount)
		{
			CreateDamageReport(in actor, in target, in targetIndex, in targetCount, out DamageReport damageReport);
			await EventManager.Call<IDamageReportListener>(async call => {
				call.OnBroadcastDamageReport(damageReport);
			});
			DestroyThis(false);
		}

		protected virtual void CreateDamageReport(in IUnitInteractiveValue actor, in IUnitInteractiveValue target, in int targetIndex, in int targetCount, out DamageReport damageReport)
		{
			damageReport = new DamageReport(actor, target);

			IUnitTypeValue ActorType = actor.TypeValueData;
			IUnitPlayValue ActorValue = actor.PlayValueData;

			IUnitTypeValue TargetType = target.TypeValueData;
			IUnitPlayValue TargetValue = target.PlayValueData;

			AttackAffinityAdvantage attackAffinityAdvantage = ComputeAttackAffinityAdvantage(ActorType.AttackTypeValue, TargetType.DefenseTypeValue);
			FieldAdvantage actorFieldAdvantage = ComputeFieldAdvantage(ActorType.FieldTypeValue, fieldType);
			FieldAdvantage targetFieldAdvantage = ComputeFieldAdvantage(TargetType.FieldTypeValue, fieldType);

			DamageType damageType = DamageType.����;
			SubDamageType subDamageType = attackAffinityAdvantage.SubDamageType;

			// ���� ����
			if(!CheckAccuracy(ActorValue.AccuracyPoint.Value * actorFieldAdvantage.accuracyProbability, TargetValue.Anti_AccuracyPoint.Value * targetFieldAdvantage.anti_accuracyProbability))
			{
				damageReport.SetReport(0, DamageType.������);
				return;
			}
			// ũ��Ƽ�� ����
			if(CheckCritical(ActorValue.CriticalProbabilityPoint.Value * actorFieldAdvantage.criticalProbability, TargetValue.Anti_CriticalProbabilityPoint.Value * targetFieldAdvantage.anti_accuracyProbability))
			{
				subDamageType |= SubDamageType.ġ��Ÿ;
			}

			///////////////////////////////////////
			// �⺻ ���ط� 
			float baseDamage = ActorValue.OffenseDamagePoint.Value * Utils.RandomDistribution.NextGaussianClamp1() * ActorValue.OffenseRandomRate.Value;
			// ���� ���ʽ�
			float fieldDamage = actorFieldAdvantage.attackDamage;
			// ġ�� ���ʽ�
			float criticalDamage = subDamageType.HasFlag(SubDamageType.ġ��Ÿ) ? Mathf.Max(ActorValue.CriticalAttackMultiplier.Value - TargetValue.Anti_CriticalAttackMultiplier.Value,0.2f) : 0f;
			// ���� ���ط�
			float resultDamage = baseDamage * (fieldDamage + criticalDamage);
			///////////////////////////////////////
			// �⺻ ��
			float anti_baseDamage = TargetValue.DefenseDamagePoint.Value * Utils.RandomDistribution.NextGaussianClamp1() * TargetValue.DefenseRandomRate.Value;
			// ���� ���ʽ�
			float anti_fieldDamage = targetFieldAdvantage.attackDamage;
			// ���� ���� 
			float anti_resultDamage = anti_baseDamage * (anti_fieldDamage);
			///////////////////////////////////////
			// �� ���ʽ�
			float affinityDamage = attackAffinityAdvantage.affinityAttackDamage;
			// ���� ���ط�
			float totalDamage = (resultDamage - anti_resultDamage) * (affinityDamage);
			///////////////////////////////////////
			// ���� ���ط�(Int)
			int totalDamageInt = Mathf.Max(Mathf.FloorToInt(totalDamage),0);
			damageReport.SetReport(totalDamageInt, damageType, subDamageType);
		}



		// ��
		protected virtual AttackAffinityAdvantage ComputeAttackAffinityAdvantage(AttackType attack, DefenseType defense)
		{
			AdvantageLevel level = ((attack, defense) switch {
				(AttackType.�Ϲ�, _) => AdvantageLevel.�Ϲ�,
				(_, DefenseType.�Ϲ�) => AdvantageLevel.�Ϲ�,

				(AttackType.����, DefenseType.����) => AdvantageLevel.��ȿ,
				(AttackType.����, DefenseType.����) => AdvantageLevel.�Ϲ�,
				(AttackType.����, DefenseType.Ư��) => AdvantageLevel.��ȿ,
				(AttackType.����, DefenseType.ź��) => AdvantageLevel.��ȿ,

				(AttackType.����, DefenseType.����) => AdvantageLevel.��ȿ,
				(AttackType.����, DefenseType.����) => AdvantageLevel.��ȿ,
				(AttackType.����, DefenseType.Ư��) => AdvantageLevel.�Ϲ�,
				(AttackType.����, DefenseType.ź��) => AdvantageLevel.�Ϲ�,

				(AttackType.�ź�, DefenseType.����) => AdvantageLevel.�Ϲ�,
				(AttackType.�ź�, DefenseType.����) => AdvantageLevel.��ȿ,
				(AttackType.�ź�, DefenseType.Ư��) => AdvantageLevel.��ȿ,
				(AttackType.�ź�, DefenseType.ź��) => AdvantageLevel.����,

				(AttackType.����, DefenseType.����) => AdvantageLevel.�Ϲ�,
				(AttackType.����, DefenseType.����) => AdvantageLevel.��ȿ,
				(AttackType.����, DefenseType.Ư��) => AdvantageLevel.����,
				(AttackType.����, DefenseType.ź��) => AdvantageLevel.��ȿ,
				_ => AdvantageLevel.�Ϲ�
			});

			return new AttackAffinityAdvantage() {
				level = level,
				affinityAttackDamage = level switch {
					AdvantageLevel.�־� => 0.25f,
					AdvantageLevel.��ȿ => 0.50f,
					AdvantageLevel.���� => 0.75f,
					AdvantageLevel.�Ϲ� => 1.00f,
					AdvantageLevel.���� => 1.50f,
					AdvantageLevel.��ȿ => 2.00f,
					AdvantageLevel.�ְ� => 2.50f,
					_ => 1f
				}
			};
		}
		protected virtual FieldAdvantage ComputeFieldAdvantage(FieldAdvantageLevel fieldAdvantageLevel, FieldAdvantageLevel.FieldType fieldType)
		{
			var level = fieldAdvantageLevel[fieldType];
			return new FieldAdvantage() {
				level = level,
				attackDamage = AdvantageAttackDamage(level),
				accuracyProbability = AdvantageAccuracy(level),
				criticalProbability = AdvantageCritical(level),

				anti_attackDamage = Anti_AdvantageAttackDamage(level),
				anti_accuracyProbability = Anti_AdvantageAccuracy(level),
				anti_criticalProbability = Anti_AdvantageCritical(level),
			};
			float AdvantageAttackDamage(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.�־� => 0.70f,
					AdvantageLevel.��ȿ => 0.80f,
					AdvantageLevel.���� => 0.90f,
					AdvantageLevel.�Ϲ� => 1.00f,
					AdvantageLevel.���� => 1.10f,
					AdvantageLevel.��ȿ => 1.20f,
					AdvantageLevel.�ְ� => 1.30f,
					_ => 1f
				};
			}
			float AdvantageAccuracy(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.�־� => -.15f,
					AdvantageLevel.��ȿ => -.10f,
					AdvantageLevel.���� => -.05f,
					AdvantageLevel.�Ϲ� => +.00f,
					AdvantageLevel.���� => +.05f,
					AdvantageLevel.��ȿ => +.10f,
					AdvantageLevel.�ְ� => +.15f,
					_ => 1f
				};
			}
			float AdvantageCritical(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.�־� => -.15f,
					AdvantageLevel.��ȿ => -.10f,
					AdvantageLevel.���� => -.05f,
					AdvantageLevel.�Ϲ� => +.00f,
					AdvantageLevel.���� => +.05f,
					AdvantageLevel.��ȿ => +.10f,
					AdvantageLevel.�ְ� => +.15f,
					_ => 1f
				};
			}

			float Anti_AdvantageAttackDamage(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.�־� => 0.70f,
					AdvantageLevel.��ȿ => 0.80f,
					AdvantageLevel.���� => 0.90f,
					AdvantageLevel.�Ϲ� => 1.00f,
					AdvantageLevel.���� => 1.10f,
					AdvantageLevel.��ȿ => 1.20f,
					AdvantageLevel.�ְ� => 1.30f,
					_ => 1f
				};
			}
			float Anti_AdvantageAccuracy(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.�־� => -.15f,
					AdvantageLevel.��ȿ => -.10f,
					AdvantageLevel.���� => -.05f,
					AdvantageLevel.�Ϲ� => +.00f,
					AdvantageLevel.���� => +.05f,
					AdvantageLevel.��ȿ => +.10f,
					AdvantageLevel.�ְ� => +.15f,
					_ => 1f
				};
			}
			float Anti_AdvantageCritical(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.�־� => -.15f,
					AdvantageLevel.��ȿ => -.10f,
					AdvantageLevel.���� => -.05f,
					AdvantageLevel.�Ϲ� => +.00f,
					AdvantageLevel.���� => +.05f,
					AdvantageLevel.��ȿ => +.10f,
					AdvantageLevel.�ְ� => +.15f,
					_ => 1f
				};
			}
		}

		protected virtual bool CheckAccuracy(float accuracy, float evasion)
		{
			return BernoulliTrial(Probability_A_in_sumAB(accuracy, evasion));
		}
		protected virtual bool CheckCritical(float criticalProbabilityPoint, float anti_criticalProbabilityPoint)
		{
			return BernoulliTrial(Probability_A_in_sumAB(criticalProbabilityPoint, anti_criticalProbabilityPoint));
		}

		/// <summary><code>
		/// ���� ���� ��ǥ�� X �� ��� (-1~1)(���� 0 ���� ��ħ)
		/// </code></summary>
		protected float RandomInCircleX()
		{
			return Random.insideUnitCircle.x;
		}
		/// <summary>
		/// Box-Muller ��ȯ�� ����Ͽ� ���� ������ ������ ������ double ���� �����մϴ�.
		/// ����� 0, ǥ�������� 1�Դϴ�.
		/// </summary>
		float GenerateNormalRandom()
		{
			float u, v, s;
			do
			{
				u = Random.value * 2.0f - 1.0f; // -1.0���� 1.0 ����
				v = Random.value * 2.0f - 1.0f; // -1.0���� 1.0 ����
				s = u * u + v * v;
			}
			while(s >= 1.0 || s == 0.0);

			s = Mathf.Sqrt((float)(-2.0 * Mathf.Log((float)s) / s));
			return u * s;
		}
		/// <summary>
		/// �߾ӱ��������� �̿��Ͽ� ���� ������ �ٻ��ϴ� ���� ���� �����մϴ�.
		/// </summary>
		/// <param name="numSamples">�ջ��� ���� ���� ���� (Ŭ���� ���� ������ �������)</param>
		public static float NextGaussian(int numSamples = 12)
		{
			float sum = 0f;
			for(int i = 0 ; i < numSamples ; i++)
			{
				sum += Random.Range(0f, 1f);
			}
			// ����� 0����, ǥ�������� 1�� ���߱� ���� ����
			return (sum - numSamples / 2f) / (Mathf.Sqrt(numSamples / 12f));
		}

		/// <summary>
		/// A�� B�� ���� ���� ��ȯ�մϴ�.
		/// ���Ѵ밡 ���Ե� ��� �����ϰ� ó���մϴ�.
		/// A / B�� 0���� ������ 0���� ��ȯ�մϴ�.
		/// </summary>
		protected float Probability_A_in_B(float A, float B)
		{
			if(A<0f) A = 0f;
			if(B<0f) B = 0f;
			if(Mathf.Approximately(A, 0f)) A = 0f;
			if(Mathf.Approximately(B, 0f)) B = 0f;
			bool isPositiveInfinity_A = float.IsPositiveInfinity(A);
			bool isPositiveInfinity_B = float.IsPositiveInfinity(B);

			//���Ѵ� ó��
			if(isPositiveInfinity_A)
			{
				return 1f;
			}
			if(isPositiveInfinity_B)
			{
				return 0f;
			}

			// 0�� ��� ó��
			return B == 0f
				? (A == 0f) ? 0.5f : 1f
				: (A == 0f) ? 0f : A / B;
		}

		/// <summary>
		/// A�� B�� �� �տ��� A�� �����ϴ� ������ ��ȯ�մϴ�.
		/// </summary>
		protected float Probability_A_in_sumAB(float A, float B)
		{
			if(A<0f) A = 0f;
			if(B<0f) B = 0f;
			bool isPositiveInfinity_A = float.IsPositiveInfinity(A);
			bool isPositiveInfinity_B = float.IsPositiveInfinity(B);

			if(isPositiveInfinity_A && isPositiveInfinity_B)
			{
				// �� �� ���Ѵ��� ��� ���ǵ��� �����Ƿ� ���ܸ� �����ų� �⺻�� ��ȯ
				return 0.5f; // �Ǵ� throw new ArgumentException("Both A and other are infinite.");
			}
			if(isPositiveInfinity_A)
			{
				// A�� ���Ѵ��̰� B�� ������ ��� A�� ������ 1
				return 1f;
			}
			if(isPositiveInfinity_B)
			{
				// B�� ���Ѵ��̰� A�� ������ ��� A�� ������ 0
				return 0f;
			}

			// A�� B�� ������ ���
			float total = A + B;

			if(Mathf.Approximately(total, 0f))
			{
				// A�� B�� ���� 0�� ����� ��� ������ 0.5�� ��ȯ
				return 0.5f;
			}
			if(Mathf.Approximately(A, B))
			{
				// A�� Other�� ���� ���� ��
				return 0.5f;
			}
			return A / total;
		}
		/// <summary>
		/// A�� other �迭�� �� �տ��� A�� �����ϴ� ������ ��ȯ�մϴ�.
		/// </summary>
		protected float Probability_A_in_Total(float A, params float[] other)
		{
			int length = other.Length;
			if(length == 1)
			{
				return Probability_A_in_sumAB(A, other[0]);
			}
			if(A<0f) A = 0f;

			// ���Ѵ� ���� �˻�
			bool isPositiveInfinity_A = float.IsPositiveInfinity(A);
			bool isPositiveInfinity_Other = false;

			// other �迭 �� ���Ѵ� ���� �˻�
			for(int o = 0 ; o < length ; o++)
			{
				if(float.IsPositiveInfinity(other[o]))
				{
					isPositiveInfinity_Other = true;
					break;
				}
			}

			if(isPositiveInfinity_A && isPositiveInfinity_Other)
			{
				// �� �� ���Ѵ��� ��� ���ǵ��� �����Ƿ� ���ܸ� �����ų� �⺻�� ��ȯ
				return 0.5f; // �Ǵ� throw new ArgumentException("Both A and other are infinite.");
			}
			if(isPositiveInfinity_A)
			{
				// A�� ���Ѵ��̰� B�� ������ ��� A�� ������ 1
				return 1f;
			}
			if(isPositiveInfinity_Other)
			{
				// other�� ���Ѵ��̰� A�� ������ ��� A�� ������ 0
				return 0f;
			}

			// ��� other�� ������ ���
			float sumOther = 0f;
			foreach(var v in other)
			{
				sumOther += v;
			}
			if(sumOther<0f) sumOther = 0f;
			float total = A + sumOther;

			// A�� Other�� ������ 0�� ��� 0.5 ��ȯ
			if(Mathf.Approximately(total, 0f))
			{
				return 0.5f;
			}

			// A�� Other�� ���հ� ���� ��
			if(Mathf.Approximately(A, sumOther))
			{
				return 0.5f;
			}

			// �Ϲ����� ��� A / (A + sumOther)
			return A / total;
		}

		/// <summary>
		/// ������ probability Ȯ���� ��ġ�� true �� ��ȯ�ϴ� �Լ�
		/// offset�� �⺻ 0 �̴�. + ���� Ȯ���� �ø���, - �� Ȯ���� �����. 
		/// probability + offset �� 1���� ũ�� ������ ���̴�.
		/// probability + offset �� 1���� ������ ������ �����̴�.
		/// </summary>
		protected bool BernoulliTrial(float probability, float offset = 0f)
		{
			return probability + offset > Random.value;
		}
	}
}
