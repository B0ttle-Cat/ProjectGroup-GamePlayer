using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamInteractiveValue : ComponentBehaviour, ITeamInteractiveValue
	{
		public IFireteamData ThisTeamData { get; set; }
		public Vector3 ThisTeamPosition { get; set; }
		public FireteamMemberCollector MemberCollector { get; set; }
		public void OnUpdateInit()
		{
			ThisContainer.AwaitGetComponent<FireteamMemberCollector>((memberCollector) => {
				this.MemberCollector = memberCollector;
			});
		}

		public void OnValueRefresh()
		{
			if(MemberCollector == null) return;

			ThisTeamPosition = MemberCollector.CenterPosition;
		}
	}
}
