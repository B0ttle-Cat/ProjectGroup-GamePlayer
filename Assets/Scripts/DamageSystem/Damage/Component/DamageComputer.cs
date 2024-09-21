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
				AdvantageLevel.최악 => SubDamageType.최악,
				AdvantageLevel.무효 => SubDamageType.무효,
				AdvantageLevel.저항 => SubDamageType.저항,
				AdvantageLevel.일반 => SubDamageType.일반,
				AdvantageLevel.증폭 => SubDamageType.증폭,
				AdvantageLevel.유효 => SubDamageType.유효,
				AdvantageLevel.최고 => SubDamageType.최고,
				_ => SubDamageType.일반,
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

			DamageType damageType = DamageType.피해;
			SubDamageType subDamageType = attackAffinityAdvantage.SubDamageType;

			// 명중 결정
			if(!CheckAccuracy(ActorValue.AccuracyPoint.Value * actorFieldAdvantage.accuracyProbability, TargetValue.Anti_AccuracyPoint.Value * targetFieldAdvantage.anti_accuracyProbability))
			{
				damageReport.SetReport(0, DamageType.빗나감);
				return;
			}
			// 크리티컬 결정
			if(CheckCritical(ActorValue.CriticalProbabilityPoint.Value * actorFieldAdvantage.criticalProbability, TargetValue.Anti_CriticalProbabilityPoint.Value * targetFieldAdvantage.anti_accuracyProbability))
			{
				subDamageType |= SubDamageType.치명타;
			}

			///////////////////////////////////////
			// 기본 피해량 
			float baseDamage = ActorValue.OffenseDamagePoint.Value * Utils.RandomDistribution.NextGaussianClamp1() * ActorValue.OffenseRandomRate.Value;
			// 지형 보너스
			float fieldDamage = actorFieldAdvantage.attackDamage;
			// 치명 보너스
			float criticalDamage = subDamageType.HasFlag(SubDamageType.치명타) ? Mathf.Max(ActorValue.CriticalAttackMultiplier.Value - TargetValue.Anti_CriticalAttackMultiplier.Value,0.2f) : 0f;
			// 전달 피해량
			float resultDamage = baseDamage * (fieldDamage + criticalDamage);
			///////////////////////////////////////
			// 기본 방어값
			float anti_baseDamage = TargetValue.DefenseDamagePoint.Value * Utils.RandomDistribution.NextGaussianClamp1() * TargetValue.DefenseRandomRate.Value;
			// 지형 보너스
			float anti_fieldDamage = targetFieldAdvantage.attackDamage;
			// 최종 방어력 
			float anti_resultDamage = anti_baseDamage * (anti_fieldDamage);
			///////////////////////////////////////
			// 상성 보너스
			float affinityDamage = attackAffinityAdvantage.affinityAttackDamage;
			// 최종 피해량
			float totalDamage = (resultDamage - anti_resultDamage) * (affinityDamage);
			///////////////////////////////////////
			// 최종 피해량(Int)
			int totalDamageInt = Mathf.Max(Mathf.FloorToInt(totalDamage),0);
			damageReport.SetReport(totalDamageInt, damageType, subDamageType);
		}



		// 상성
		protected virtual AttackAffinityAdvantage ComputeAttackAffinityAdvantage(AttackType attack, DefenseType defense)
		{
			AdvantageLevel level = ((attack, defense) switch {
				(AttackType.일반, _) => AdvantageLevel.일반,
				(_, DefenseType.일반) => AdvantageLevel.일반,

				(AttackType.폭발, DefenseType.경장) => AdvantageLevel.유효,
				(AttackType.폭발, DefenseType.중장) => AdvantageLevel.일반,
				(AttackType.폭발, DefenseType.특수) => AdvantageLevel.무효,
				(AttackType.폭발, DefenseType.탄력) => AdvantageLevel.무효,

				(AttackType.관통, DefenseType.경장) => AdvantageLevel.무효,
				(AttackType.관통, DefenseType.중장) => AdvantageLevel.유효,
				(AttackType.관통, DefenseType.특수) => AdvantageLevel.일반,
				(AttackType.관통, DefenseType.탄력) => AdvantageLevel.일반,

				(AttackType.신비, DefenseType.경장) => AdvantageLevel.일반,
				(AttackType.신비, DefenseType.중장) => AdvantageLevel.무효,
				(AttackType.신비, DefenseType.특수) => AdvantageLevel.유효,
				(AttackType.신비, DefenseType.탄력) => AdvantageLevel.증폭,

				(AttackType.진동, DefenseType.경장) => AdvantageLevel.일반,
				(AttackType.진동, DefenseType.중장) => AdvantageLevel.무효,
				(AttackType.진동, DefenseType.특수) => AdvantageLevel.증폭,
				(AttackType.진동, DefenseType.탄력) => AdvantageLevel.유효,
				_ => AdvantageLevel.일반
			});

			return new AttackAffinityAdvantage() {
				level = level,
				affinityAttackDamage = level switch {
					AdvantageLevel.최악 => 0.25f,
					AdvantageLevel.무효 => 0.50f,
					AdvantageLevel.저항 => 0.75f,
					AdvantageLevel.일반 => 1.00f,
					AdvantageLevel.증폭 => 1.50f,
					AdvantageLevel.유효 => 2.00f,
					AdvantageLevel.최고 => 2.50f,
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
					AdvantageLevel.최악 => 0.70f,
					AdvantageLevel.무효 => 0.80f,
					AdvantageLevel.저항 => 0.90f,
					AdvantageLevel.일반 => 1.00f,
					AdvantageLevel.증폭 => 1.10f,
					AdvantageLevel.유효 => 1.20f,
					AdvantageLevel.최고 => 1.30f,
					_ => 1f
				};
			}
			float AdvantageAccuracy(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.최악 => -.15f,
					AdvantageLevel.무효 => -.10f,
					AdvantageLevel.저항 => -.05f,
					AdvantageLevel.일반 => +.00f,
					AdvantageLevel.증폭 => +.05f,
					AdvantageLevel.유효 => +.10f,
					AdvantageLevel.최고 => +.15f,
					_ => 1f
				};
			}
			float AdvantageCritical(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.최악 => -.15f,
					AdvantageLevel.무효 => -.10f,
					AdvantageLevel.저항 => -.05f,
					AdvantageLevel.일반 => +.00f,
					AdvantageLevel.증폭 => +.05f,
					AdvantageLevel.유효 => +.10f,
					AdvantageLevel.최고 => +.15f,
					_ => 1f
				};
			}

			float Anti_AdvantageAttackDamage(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.최악 => 0.70f,
					AdvantageLevel.무효 => 0.80f,
					AdvantageLevel.저항 => 0.90f,
					AdvantageLevel.일반 => 1.00f,
					AdvantageLevel.증폭 => 1.10f,
					AdvantageLevel.유효 => 1.20f,
					AdvantageLevel.최고 => 1.30f,
					_ => 1f
				};
			}
			float Anti_AdvantageAccuracy(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.최악 => -.15f,
					AdvantageLevel.무효 => -.10f,
					AdvantageLevel.저항 => -.05f,
					AdvantageLevel.일반 => +.00f,
					AdvantageLevel.증폭 => +.05f,
					AdvantageLevel.유효 => +.10f,
					AdvantageLevel.최고 => +.15f,
					_ => 1f
				};
			}
			float Anti_AdvantageCritical(AdvantageLevel level)
			{
				return level switch {
					AdvantageLevel.최악 => -.15f,
					AdvantageLevel.무효 => -.10f,
					AdvantageLevel.저항 => -.05f,
					AdvantageLevel.일반 => +.00f,
					AdvantageLevel.증폭 => +.05f,
					AdvantageLevel.유효 => +.10f,
					AdvantageLevel.최고 => +.15f,
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
		/// 원형 랜덤 좌표의 X 값 사용 (-1~1)(고르게 0 으로 뭉침)
		/// </code></summary>
		protected float RandomInCircleX()
		{
			return Random.insideUnitCircle.x;
		}
		/// <summary>
		/// Box-Muller 변환을 사용하여 정규 분포를 따르는 랜덤한 double 값을 생성합니다.
		/// 평균은 0, 표준편차는 1입니다.
		/// </summary>
		float GenerateNormalRandom()
		{
			float u, v, s;
			do
			{
				u = Random.value * 2.0f - 1.0f; // -1.0에서 1.0 사이
				v = Random.value * 2.0f - 1.0f; // -1.0에서 1.0 사이
				s = u * u + v * v;
			}
			while(s >= 1.0 || s == 0.0);

			s = Mathf.Sqrt((float)(-2.0 * Mathf.Log((float)s) / s));
			return u * s;
		}
		/// <summary>
		/// 중앙극한정리를 이용하여 정규 분포를 근사하는 랜덤 값을 생성합니다.
		/// </summary>
		/// <param name="numSamples">합산할 랜덤 값의 개수 (클수록 정규 분포에 가까워짐)</param>
		public static float NextGaussian(int numSamples = 12)
		{
			float sum = 0f;
			for(int i = 0 ; i < numSamples ; i++)
			{
				sum += Random.Range(0f, 1f);
			}
			// 평균을 0으로, 표준편차를 1로 맞추기 위해 조정
			return (sum - numSamples / 2f) / (Mathf.Sqrt(numSamples / 12f));
		}

		/// <summary>
		/// A를 B로 나눈 값을 반환합니다.
		/// 무한대가 포함된 경우 적절하게 처리합니다.
		/// A / B가 0보다 작으면 0으로 반환합니다.
		/// </summary>
		protected float Probability_A_in_B(float A, float B)
		{
			if(A<0f) A = 0f;
			if(B<0f) B = 0f;
			if(Mathf.Approximately(A, 0f)) A = 0f;
			if(Mathf.Approximately(B, 0f)) B = 0f;
			bool isPositiveInfinity_A = float.IsPositiveInfinity(A);
			bool isPositiveInfinity_B = float.IsPositiveInfinity(B);

			//무한대 처리
			if(isPositiveInfinity_A)
			{
				return 1f;
			}
			if(isPositiveInfinity_B)
			{
				return 0f;
			}

			// 0인 경우 처리
			return B == 0f
				? (A == 0f) ? 0.5f : 1f
				: (A == 0f) ? 0f : A / B;
		}

		/// <summary>
		/// A와 B의 총 합에서 A가 차지하는 비율을 반환합니다.
		/// </summary>
		protected float Probability_A_in_sumAB(float A, float B)
		{
			if(A<0f) A = 0f;
			if(B<0f) B = 0f;
			bool isPositiveInfinity_A = float.IsPositiveInfinity(A);
			bool isPositiveInfinity_B = float.IsPositiveInfinity(B);

			if(isPositiveInfinity_A && isPositiveInfinity_B)
			{
				// 둘 다 무한대인 경우 정의되지 않으므로 예외를 던지거나 기본값 반환
				return 0.5f; // 또는 throw new ArgumentException("Both A and other are infinite.");
			}
			if(isPositiveInfinity_A)
			{
				// A가 무한대이고 B는 유한한 경우 A의 비율은 1
				return 1f;
			}
			if(isPositiveInfinity_B)
			{
				// B가 무한대이고 A는 유한한 경우 A의 비율은 0
				return 0f;
			}

			// A와 B가 유한한 경우
			float total = A + B;

			if(Mathf.Approximately(total, 0f))
			{
				// A와 B의 합이 0에 가까운 경우 비율을 0.5로 반환
				return 0.5f;
			}
			if(Mathf.Approximately(A, B))
			{
				// A와 Other가 값이 같을 때
				return 0.5f;
			}
			return A / total;
		}
		/// <summary>
		/// A와 other 배열의 총 합에서 A가 차지하는 비율을 반환합니다.
		/// </summary>
		protected float Probability_A_in_Total(float A, params float[] other)
		{
			int length = other.Length;
			if(length == 1)
			{
				return Probability_A_in_sumAB(A, other[0]);
			}
			if(A<0f) A = 0f;

			// 무한대 여부 검사
			bool isPositiveInfinity_A = float.IsPositiveInfinity(A);
			bool isPositiveInfinity_Other = false;

			// other 배열 내 무한대 여부 검사
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
				// 둘 다 무한대인 경우 정의되지 않으므로 예외를 던지거나 기본값 반환
				return 0.5f; // 또는 throw new ArgumentException("Both A and other are infinite.");
			}
			if(isPositiveInfinity_A)
			{
				// A가 무한대이고 B는 유한한 경우 A의 비율은 1
				return 1f;
			}
			if(isPositiveInfinity_Other)
			{
				// other가 무한대이고 A는 유한한 경우 A의 비율은 0
				return 0f;
			}

			// 모든 other가 유한한 경우
			float sumOther = 0f;
			foreach(var v in other)
			{
				sumOther += v;
			}
			if(sumOther<0f) sumOther = 0f;
			float total = A + sumOther;

			// A와 Other의 총합이 0인 경우 0.5 반환
			if(Mathf.Approximately(total, 0f))
			{
				return 0.5f;
			}

			// A와 Other의 총합과 같을 때
			if(Mathf.Approximately(A, sumOther))
			{
				return 0.5f;
			}

			// 일반적인 경우 A / (A + sumOther)
			return A / total;
		}

		/// <summary>
		/// 지정된 probability 확률로 수치로 true 를 반환하는 함수
		/// offset의 기본 0 이다. + 값은 확률을 올리며, - 는 확률을 낮춘다. 
		/// probability + offset 가 1보다 크면 무조건 참이다.
		/// probability + offset 가 1보다 작으면 무조건 거짓이다.
		/// </summary>
		protected bool BernoulliTrial(float probability, float offset = 0f)
		{
			return probability + offset > Random.value;
		}
	}
}
