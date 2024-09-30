using System;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[Serializable]
	public partial class StartUnitSettingCharacter : IFireunitData
	{
		[SerializeField, HideInInspector] private CharacterSetter characterSetter;
		[SerializeField, HideInInspector] private Vector3Int fireunitData;

		[ShowInInspector, PropertyOrder(1), FoldoutGroup("CharacterSetter"), HideLabel, HideReferenceObjectPicker]
		public CharacterSetter CharacterSetter { get => characterSetter; set => characterSetter = value; }
		public Vector3Int MemberUniqueID => fireunitData;


		[HorizontalGroup("MemberUniqueID", order: -1), ShowInInspector, ValueDropdown("ShowTargetFactionName"), LabelText("Member")]
		public int FactionIndex { get => fireunitData.x; set => fireunitData.x = value; }

		[HorizontalGroup("MemberUniqueID", width: 0.2f), ShowInInspector, ValueDropdown("ShowTargetTeamIndex"), HideLabel]
		public int TeamIndex { get => fireunitData.y; set => fireunitData.y = value; }

		[HorizontalGroup("MemberUniqueID", width: 0.2f), ShowInInspector, ValueDropdown("ShowTargetUnitIndex"), HideLabel]
		public int UnitIndex { get => fireunitData.z; set => fireunitData.z = value; }


		public void Dispose()
		{

		}
	}
}
