using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;

namespace BC.Character
{
	public class CharacterData : DataObject, IFireunitData, ICharacterResourcesData
	{
		[SerializeField]
		[ValueDropdown("ShowTargetFactionName")]
		private int factionIndex;
		[SerializeField]
		private int fireteamIndex;
		[SerializeField]
		private int fireunitIndex;

		[SerializeField, HideLabel]
		[FoldoutGroup("CharacterModel")]
		private ResourcesKey characterKey;
		[SerializeField, HideLabel]
		[FoldoutGroup("WeaponeModel")]
		private ResourcesKey weaponeKey;

		public int MemberUniqueID { get; set; }
		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
		public int TeamIndex { get => fireteamIndex; set => fireteamIndex = value; }
		public int UnitIndex { get => fireunitIndex; set => fireunitIndex = value; }

		public ResourcesKey CharacterResourcesKey { get => characterKey; set => characterKey = value; }
		public ResourcesKey WeaponResourcesKey { get => weaponeKey; set => weaponeKey = value; }

		public bool IsEqualsUnit(int faction, int team, int unit)
		{
			IFireunitData thisUnitData = this;
			if (thisUnitData == null) return false;
			return thisUnitData.IsEqualsTeam(faction, team) && UnitIndex == unit;
		}
		public bool IsEqualsUnit(CharacterData other)
		{
			IFireunitData thisUnitData = this;
			IFireunitData otherUnitData = other;
			if (thisUnitData == null || otherUnitData == null) return false;
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
