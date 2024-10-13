using System;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IUnitTypeValueData : IOdccData, IUnitTypeValue
	{

	}

	public interface IUnitTypeValue : IOdccData
	{
		AttackType ����Ÿ�� { get; set; }
		DefenseType ���Ÿ�� { get; set; }

		SubAttackType ��������Ÿ�� { get; set; }
		SubDefenseType �������Ÿ�� { get; set; }

		WeaponType ����з� { get; set; }
		PositionType �����з� { get; set; }
		RoleType ���Һз� { get; set; }
		TierType ��޺з� { get; set; }

		FieldAdvantageLevel �����Ӽ� { get; set; }
		[EnumPaging]
		enum AdvantageLevel
		{
			�־� = -30,
			��ȿ = -20,
			���� = -10,
			�Ϲ� = 00,
			���� = 10,
			��ȿ = 20,
			�ְ� = 30,
		}
		[EnumToggleButtons]
		enum AttackType
		{
			�Ϲ�,
			����,
			����,
			�ź�,
			����,
		}
		[EnumToggleButtons]
		enum DefenseType
		{
			�Ϲ�,
			����,
			����,
			Ư��,
			ź��,
		}
		[EnumToggleButtons]
		enum SubAttackType
		{
			����,
			�빰,
			������,
			����,
		}
		[EnumToggleButtons]
		enum SubDefenseType
		{
			����,
			����,
			�Ⱙ,
			������
		}
		enum WeaponType
		{
			����,
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
		[EnumToggleButtons]
		enum PositionType
		{
			����,
			�߿�,
			�Ŀ�,
			����,
		}
		[EnumToggleButtons]
		enum RoleType
		{
			����,
			��Ŀ,
			����,
			������,
		}

		[EnumPaging]
		enum TierType
		{
			��� = 00,    // ���� ����
			��� = 10,    // ��� ����
			Ư�� = 20,    // Ư�� ����
			���� = 30,    // ���� ����
			���� = 40,    // ���� ����
		}

		[Serializable]
		public struct FieldAdvantageLevel
		{
			public enum FieldType
			{
				������ = 0,
				�ð���,
				����,
				�ǳ�
			}
			[LabelText("�����Ӽ�-�ð���")]
			public AdvantageLevel �ð���;
			[LabelText("�����Ӽ�-����")]
			public AdvantageLevel ����;
			[LabelText("�����Ӽ�-�ǳ�")]
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
						case FieldType.����: ���� = value; break;
						case FieldType.�ǳ�: �ǳ� = value; break;
					}
				}
			}
		}

		public void Paste(IUnitTypeValue value)
		{
			����Ÿ�� = value.����Ÿ��;
			���Ÿ�� = value.���Ÿ��;
			��������Ÿ�� = value.��������Ÿ��;
			�������Ÿ�� = value.�������Ÿ��;
			����з� = value.����з�;
			�����з� = value.�����з�;
			���Һз� = value.���Һз�;
			��޺з� = value.��޺з�;
			�����Ӽ� = value.�����Ӽ�;
		}
	}

	public class FireunitTypeValue : DataObject, IUnitTypeValueData
	{
		[SerializeField] private IUnitTypeValue.AttackType _����Ÿ��;
		[SerializeField] private IUnitTypeValue.DefenseType _���Ÿ��;
		[SerializeField] private IUnitTypeValue.SubAttackType _��������Ÿ��;
		[SerializeField] private IUnitTypeValue.SubDefenseType _�������Ÿ��;
		[SerializeField] private IUnitTypeValue.WeaponType _����з�;
		[SerializeField] private IUnitTypeValue.PositionType _�����з�;
		[SerializeField] private IUnitTypeValue.RoleType _���Һз�;
		[SerializeField] private IUnitTypeValue.TierType _��޺з�;
		[SerializeField] private IUnitTypeValue.FieldAdvantageLevel _�����Ӽ�;

		public IUnitTypeValue.AttackType ����Ÿ�� { get => _����Ÿ��; set => _����Ÿ��=value; }
		public IUnitTypeValue.DefenseType ���Ÿ�� { get => _���Ÿ��; set => _���Ÿ��=value; }
		public IUnitTypeValue.SubAttackType ��������Ÿ�� { get => _��������Ÿ��; set => _��������Ÿ��=value; }
		public IUnitTypeValue.SubDefenseType �������Ÿ�� { get => _�������Ÿ��; set => _�������Ÿ��=value; }
		public IUnitTypeValue.WeaponType ����з� { get => _����з�; set => _����з�=value; }
		public IUnitTypeValue.PositionType �����з� { get => _�����з�; set => _�����з�=value; }
		public IUnitTypeValue.RoleType ���Һз� { get => _���Һз�; set => _���Һз�=value; }
		public IUnitTypeValue.TierType ��޺з� { get => _��޺з�; set => _��޺з�=value; }
		public IUnitTypeValue.FieldAdvantageLevel �����Ӽ� { get => _�����Ӽ�; set => _�����Ӽ�=value; }
	}
}