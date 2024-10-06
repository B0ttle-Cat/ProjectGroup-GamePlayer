using System.Collections;

using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.Character
{
	public class CharacterData : DataObject, IFireunitData
	{
		private Vector3Int fireunitData;

		[SerializeField, HideLabel]
		[FoldoutGroup("CharacterModel")]
		private ResourcesKey characterKey;
		[SerializeField, HideLabel]
		[FoldoutGroup("WeaponeModel")]
		private ResourcesKey weaponeKey;

		public Vector3Int MemberUniqueID => fireunitData;
		[ShowInInspector, ValueDropdown("ShowTargetFactionName")]
		public int FactionIndex { get => fireunitData.x; set => fireunitData.x = value; }
		[ShowInInspector]
		public int TeamIndex { get => fireunitData.y; set => fireunitData.y = value; }
		[ShowInInspector]
		public int UnitIndex { get => fireunitData.z; set => fireunitData.z = value; }

		public ResourcesKey CharacterResourcesKey { get => characterKey; set => characterKey = value; }
		public ResourcesKey WeaponResourcesKey { get => weaponeKey; set => weaponeKey = value; }

		public bool IsEqualsUnit(int faction, int team, int unit)
		{
			IFireunitData thisUnitData = this;
			if(thisUnitData == null) return false;
			return thisUnitData.IsEqualsTeam(faction, team) && UnitIndex == unit;
		}
		public bool IsEqualsUnit(CharacterData other)
		{
			IFireunitData thisUnitData = this;
			IFireunitData otherUnitData = other;
			if(thisUnitData == null || otherUnitData == null) return false;
			return thisUnitData.IsEqualsTeam(otherUnitData) && UnitIndex == otherUnitData.UnitIndex;
		}
#if UNITY_EDITOR
		public static IEnumerable ShowTargetFactionName()
		{
			return null;// FriendshipItem.ShowTargetFactionName();
		}
#endif
	}
}
