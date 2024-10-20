using System;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	[Serializable]
	public class CharacterTypeSetting : DataObject, IUnitTypeValue
	{
		[SerializeField] private IUnitTypeValue.AttackType _����Ÿ��;
		[SerializeField] private IUnitTypeValue.DefenseType _���Ÿ��;
		[SerializeField] private IUnitTypeValue.SubAttackType _��������Ÿ��;
		[SerializeField] private IUnitTypeValue.SubDefenseType _�������Ÿ��;
		[SerializeField] private IUnitTypeValue.WeaponType _����з�;
		[SerializeField] private IUnitTypeValue.PositionType _�����з�;
		[SerializeField] private IUnitTypeValue.RoleType _���Һз�;
		[SerializeField] private IUnitTypeValue.TierType _��޺з�;
		[SerializeField, InlineProperty, HideLabel] private IUnitTypeValue.FieldAdvantageLevel _�����Ӽ�;

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
