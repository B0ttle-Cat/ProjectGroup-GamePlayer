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
		[SerializeField] private IUnitTypeValue.AttackType _공격타입;
		[SerializeField] private IUnitTypeValue.DefenseType _방어타입;
		[SerializeField] private IUnitTypeValue.SubAttackType _보조공격타입;
		[SerializeField] private IUnitTypeValue.SubDefenseType _보조방어타입;
		[SerializeField] private IUnitTypeValue.WeaponType _무장분류;
		[SerializeField] private IUnitTypeValue.PositionType _진영분류;
		[SerializeField] private IUnitTypeValue.RoleType _역할분류;
		[SerializeField] private IUnitTypeValue.TierType _등급분류;
		[SerializeField, InlineProperty, HideLabel] private IUnitTypeValue.FieldAdvantageLevel _지형속성;

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
