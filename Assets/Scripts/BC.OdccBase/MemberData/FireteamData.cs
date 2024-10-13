using System.Collections;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IFireteamIndex : IFactionIndex
	{
		public int TeamIndex { get; set; }
		public bool IsEqualsTeam(int faction, int team)
		{
			return IsEqualsFaction(faction) && TeamIndex == team;
		}
		public bool IsEqualsTeam(IFireteamIndex fireteamData)
		{
			return IsEqualsFaction(fireteamData) && TeamIndex == fireteamData.TeamIndex;
		}
		public bool IsEqualsTeam(FireteamData fireteamData)
		{
			return IsEqualsFaction(fireteamData) && TeamIndex == fireteamData.TeamIndex;
		}
	}
	public interface IFireteamData : IFireteamIndex, IFactionData
	{

	}
	public class FireteamData : MemberData, IFireteamData
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
			//	GameObject.FindObjectOfType<FactionData>


			return null;// FriendshipItem.ShowTargetFactionName();
		}
#endif
	}
}
