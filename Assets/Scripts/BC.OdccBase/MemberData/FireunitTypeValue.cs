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
			�־� = -3,
			��ȿ = -2,
			���� = -1,
			�Ϲ� = 0,
			���� = 1,
			��ȿ = 2,
			�ְ� = 3,
		}
		enum AttackType
		{
			�Ϲ�,
			����,
			����,
			�ź�,
			����,
		}
		enum DefenseType
		{
			�Ϲ�,
			����,
			����,
			Ư��,
			ź��,
		}

		enum SubAttackType
		{
			����,
			�빰,
			������,
			����,
		}
		enum SubDefenseType
		{
			�Ϲ�,
			����,
			�Ⱙ,
			������
		}

		enum WeaponType
		{
			// ��ȭ��
			����,
			�������,
			���ݼ���,
			������,
			��ź��,
			// ��ȭ��
			�����,
			�ڰ���,
			ȭ������,
			��ź�߻��,
			���Ϲ߻��,
			������,
			// ����ȭ��
			�����,
			������,
			�̻���,
		}
		enum PositionType
		{
			����,
			�߿�,
			�Ŀ�,
			����,
		}
		enum RoleType
		{
			����,
			��Ŀ,
			����,
			������,
			���ȭ,
		}

		public struct FieldAdvantageLevel
		{
			public enum FieldType
			{
				������ = 0,
				�ð���,
				����,
				�ǳ�
			}
			public AdvantageLevel �ð���;
			public AdvantageLevel ����;
			public AdvantageLevel �ǳ�;

			public AdvantageLevel this[FieldType fieldType] {
				get {
					return fieldType switch {
						FieldType.�ð��� => �ð���,
						FieldType.���� => ����,
						FieldType.�ǳ� => �ǳ�,
						_ => AdvantageLevel.�Ϲ�,
					};
				}
				set {
					switch(fieldType)
					{
						case FieldType.�ð���: �ð��� = value; break;
						case FieldType.����: �ð��� = value; break;
						case FieldType.�ǳ�: �ð��� = value; break;
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
