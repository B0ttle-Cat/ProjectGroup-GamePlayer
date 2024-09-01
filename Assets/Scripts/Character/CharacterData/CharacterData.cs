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

		public int MemberUniqueID { get => 1000000 + (FactionIndex * 010000) + (TeamIndex * 000100) + (UnitIndex); }
		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
		public int TeamIndex { get => fireteamIndex; set => fireteamIndex = value; }
		public int UnitIndex { get => fireunitIndex; set => fireunitIndex = value; }

		public ResourcesKey CharacterResourcesKey { get => characterKey; set => characterKey = value; }

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
