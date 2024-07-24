using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamInteractiveValue : MemberInteractiveValue, ITeamInteractiveValue
	{
		public IFireteamData ThisTeamData { get; set; }
		public Vector3 ThisTeamPosition { get; set; }
		public FireteamMemberCollector MemberCollector { get; set; }
		public override void OnUpdateInit()
		{
			ThisContainer.AwaitGetComponent<FireteamMemberCollector>((memberCollector) => {
				this.MemberCollector = memberCollector;
			});
		}

		public override void OnValueRefresh()
		{
			if(MemberCollector == null) return;

			ThisTeamPosition = MemberCollector.CenterPosition;
		}

		public override void IsAfterValueUpdate()
		{
			if(!TryMemberTargetList(out var targetToList)) return;

			foreach(var item in targetToList)
			{
				var interactiveValue = item.Key as ITeamInteractiveValue;
				var interactiveInfo = item.Value as TeamInteractiveInfo;
				if(interactiveValue == null || interactiveInfo == null) continue;

				var fireteamData = interactiveValue.ThisTeamData;

			}
		}
	}
}
