using System;

using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	public interface IUnitTypeValue : IOdccData
	{
		AttackType 공격타입 { get; set; }
		DefenseType 방어타입 { get; set; }

		SubAttackType 보조공격타입 { get; set; }
		SubDefenseType 보조방어타입 { get; set; }

		WeaponType 무장분류 { get; set; }
		PositionType 진영분류 { get; set; }
		RoleType 역할분류 { get; set; }
		TierType 등급분류 { get; set; }

		FieldAdvantageLevel 지형속성 { get; set; }
		[EnumPaging]
		enum AdvantageLevel
		{
			최악 = -30,
			무효 = -20,
			저항 = -10,
			일반 = 00,
			증폭 = 10,
			유효 = 20,
			최고 = 30,
		}
		[EnumToggleButtons]
		enum AttackType
		{
			일반,
			폭발,
			관통,
			신비,
			진동,
		}
		[EnumToggleButtons]
		enum DefenseType
		{
			일반,
			경장,
			중장,
			특수,
			탄력,
		}
		[EnumToggleButtons]
		enum SubAttackType
		{
			대인,
			대물,
			대전차,
			공성,
		}
		[EnumToggleButtons]
		enum SubDefenseType
		{
			인형,
			차량,
			기갑,
			구조물
		}
		enum WeaponType
		{
			비무장,
			// 소화기
			권총,
			기관단총,
			돌격소총,
			저격총,
			산탄총,
			// 중화기
			기관총,
			박격포,
			화염방사기,
			유탄발사기,
			로켓발사기,
			전자포,
			// 차량화기
			기관포,
			전차포,
			미사일,
		}
		[EnumToggleButtons]
		enum PositionType
		{
			전열,
			중열,
			후열,
			보조,
		}
		[EnumToggleButtons]
		enum RoleType
		{
			딜러,
			탱커,
			힐러,
			서포터,
		}

		[EnumPaging]
		enum TierType
		{
			평범 = 00,    // 보통 유닛
			우수 = 10,    // 우수 유닛
			특수 = 20,    // 특수 유닛
			보스 = 30,    // 보스 유닛
		}

		[Serializable]
		public struct FieldAdvantageLevel
		{
			public enum FieldType
			{
				미지정 = 0,
				시가지,
				야전,
				실내
			}
			[LabelText("지형속성-시가지")]
			public AdvantageLevel 시가지;
			[LabelText("지형속성-야전")]
			public AdvantageLevel 야전;
			[LabelText("지형속성-실내")]
			public AdvantageLevel 실내;

			public AdvantageLevel this[FieldType fieldType] {
				get {
					return fieldType switch {
						FieldType.시가지 => 시가지,
						FieldType.야전 => 야전,
						FieldType.실내 => 실내,
						_ => AdvantageLevel.일반,
					};
				}
				set {
					switch(fieldType)
					{
						case FieldType.시가지: 시가지 = value; break;
						case FieldType.야전: 시가지 = value; break;
						case FieldType.실내: 시가지 = value; break;
					}
				}
			}
		}
	}

	public class FireunitTypeValue : DataObject, IUnitTypeValue
	{
		private IUnitTypeValue.AttackType _공격타입;
		private IUnitTypeValue.DefenseType _방어타입;
		private IUnitTypeValue.SubAttackType _보조공격타입;
		private IUnitTypeValue.SubDefenseType _보조방어타입;
		private IUnitTypeValue.WeaponType _무장분류;
		private IUnitTypeValue.PositionType _진영분류;
		private IUnitTypeValue.RoleType _역할분류;
		private IUnitTypeValue.TierType _등급분류;
		private IUnitTypeValue.FieldAdvantageLevel _지형속성;

		public IUnitTypeValue.AttackType 공격타입 { get => _공격타입; set => _공격타입=value; }
		public IUnitTypeValue.DefenseType 방어타입 { get => _방어타입; set => _방어타입=value; }
		public IUnitTypeValue.SubAttackType 보조공격타입 { get => _보조공격타입; set => _보조공격타입=value; }
		public IUnitTypeValue.SubDefenseType 보조방어타입 { get => _보조방어타입; set => _보조방어타입=value; }
		public IUnitTypeValue.WeaponType 무장분류 { get => _무장분류; set => _무장분류=value; }
		public IUnitTypeValue.PositionType 진영분류 { get => _진영분류; set => _진영분류=value; }
		public IUnitTypeValue.RoleType 역할분류 { get => _역할분류; set => _역할분류=value; }
		public IUnitTypeValue.TierType 등급분류 { get => _등급분류; set => _등급분류=value; }
		public IUnitTypeValue.FieldAdvantageLevel 지형속성 { get => _지형속성; set => _지형속성=value; }
	}
}
