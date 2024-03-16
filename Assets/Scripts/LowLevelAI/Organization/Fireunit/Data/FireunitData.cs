using System.Collections;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public interface IFireunitData : IOdccData, IFireteamData
	{
		public int UnitIndex { get; set; }
		public bool IsEqualsUnit(IFireunitData fireunitData)
		{
			return IsEqualsTeam(fireunitData) && UnitIndex == fireunitData.UnitIndex;
		}
		public bool IsEqualsUnit(FireunitData fireunitData)
		{
			return IsEqualsTeam(fireunitData) && UnitIndex == fireunitData.UnitIndex;
		}
	}
	public class FireunitData : DataObject, IFireunitData
	{
		[SerializeField]
		[ValueDropdown("ShowTargetFactionName")]
		private int factionIndex;
		[SerializeField]
		private int fireteamIndex;
		[SerializeField]
		private int fireunitIndex;

		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
		public int TeamIndex { get => fireteamIndex; set => fireteamIndex=value; }
		public int UnitIndex { get => fireunitIndex; set => fireunitIndex = value; }

#if UNITY_EDITOR
		public static IEnumerable ShowTargetFactionName()
		{
			return FriendshipItem.ShowTargetFactionName();
		}
#endif
	}
}
