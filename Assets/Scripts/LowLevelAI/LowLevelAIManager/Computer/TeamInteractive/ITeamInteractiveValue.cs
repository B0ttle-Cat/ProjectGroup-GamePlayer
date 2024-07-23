using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public interface ITeamInteractiveValue : IMemberInteractiveValue
	{
		public IFireteamData ThisTeamData { get; set; }
		public Vector3 ThisTeamPosition { get; set; }
		public FireteamMemberCollector MemberCollector { get; set; }
	}
}
