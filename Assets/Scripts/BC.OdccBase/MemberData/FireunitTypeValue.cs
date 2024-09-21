using BC.ODCC;

namespace BC.OdccBase
{
	public interface IUnitTypeValue : IOdccData
	{
		AttackType AttackTypeValue { get; set; }
		DefenseType DefenseTypeValue { get; set; }

		SubAttackType SubAttackTypeValue { get; set; }
		SubDefenseType SubDefenseTypeValue { get; set; }

		WeaponType WeaponTypeValue { get; set; }
		PositionType PositionTypeValue { get; set; }
		RoleType RoleTypeValue { get; set; }

		FieldAdvantageLevel FieldTypeValue { get; set; }

		enum AdvantageLevel
		{
			최악 = -3,
			무효 = -2,
			저항 = -1,
			일반 = 0,
			증폭 = 1,
			유효 = 2,
			최고 = 3,
		}
		enum AttackType
		{
			일반,
			폭발,
			관통,
			신비,
			진동,
		}
		enum DefenseType
		{
			일반,
			경장,
			중장,
			특수,
			탄력,
		}

		enum SubAttackType
		{
			대인,
			대물,
			대전차,
			공성,
		}
		enum SubDefenseType
		{
			일반,
			차량,
			기갑,
			구조물
		}

		enum WeaponType
		{
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
		enum PositionType
		{
			전열,
			중열,
			후열,
			보조,
		}
		enum RoleType
		{
			딜러,
			탱커,
			힐러,
			서포터,
			기계화,
		}

		public struct FieldAdvantageLevel
		{
			public enum FieldType
			{
				미지정 = 0,
				시가지,
				야전,
				실내
			}
			public AdvantageLevel 시가지;
			public AdvantageLevel 야전;
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

		private IUnitTypeValue.AttackType attackTypeValue;
		private IUnitTypeValue.DefenseType defenseTypeValue;
		private IUnitTypeValue.SubAttackType subAttackTypeValue;
		private IUnitTypeValue.SubDefenseType subDefenseTypeValue;
		private IUnitTypeValue.WeaponType weaponTypeValue;
		private IUnitTypeValue.PositionType positionTypeValue;
		private IUnitTypeValue.RoleType roleTypeValue;
		private IUnitTypeValue.FieldAdvantageLevel fieldTypeValue;

		public IUnitTypeValue.AttackType AttackTypeValue { get => attackTypeValue; set => attackTypeValue=value; }
		public IUnitTypeValue.DefenseType DefenseTypeValue { get => defenseTypeValue; set => defenseTypeValue=value; }
		public IUnitTypeValue.SubAttackType SubAttackTypeValue { get => subAttackTypeValue; set => subAttackTypeValue=value; }
		public IUnitTypeValue.SubDefenseType SubDefenseTypeValue { get => subDefenseTypeValue; set => subDefenseTypeValue=value; }
		public IUnitTypeValue.WeaponType WeaponTypeValue { get => weaponTypeValue; set => weaponTypeValue=value; }
		public IUnitTypeValue.PositionType PositionTypeValue { get => positionTypeValue; set => positionTypeValue=value; }
		public IUnitTypeValue.RoleType RoleTypeValue { get => roleTypeValue; set => roleTypeValue=value; }
		public IUnitTypeValue.FieldAdvantageLevel FieldTypeValue { get => fieldTypeValue; set => fieldTypeValue=value; }
	}
}
