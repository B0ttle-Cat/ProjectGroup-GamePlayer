using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamCommandData : EventCommandData
	{
		[SerializeField]
		private FireteamMemberCollector fireteamMembers;
		public FireteamMemberCollector Members { get => fireteamMembers; set => fireteamMembers = value; }
	}
}
