using System.Collections;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public interface IFireteamData : IOdccItem, IFactionData
	{
		public int TeamIndex { get; set; }
		public bool IsEqualsTeam(IFireteamData fireteamData)
		{
			return IsEqualsFaction(fireteamData) && TeamIndex == fireteamData.TeamIndex;
		}
		public bool IsEqualsTeam(FireteamData fireteamData)
		{
			return IsEqualsFaction(fireteamData) && TeamIndex == fireteamData.TeamIndex;
		}
	}
	public class FireteamData : DataObject, IFireteamData
	{
		[SerializeField]
		[ValueDropdown("ShowTargetFactionName")]
		private int factionIndex;
		[SerializeField]
		private int fireteamIndex;

		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
		public int TeamIndex { get => fireteamIndex; set => fireteamIndex=value; }

#if UNITY_EDITOR
		public static IEnumerable ShowTargetFactionName()
		{
			return FriendshipItem.ShowTargetFactionName();
		}
#endif
	}
}
