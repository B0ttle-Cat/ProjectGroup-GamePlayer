using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamController : ComponentBehaviour
	{
		private FireteamData fireteamData;
		private FireunitGrouping fireunitGrouping;

		public override void BaseAwake()
		{
			base.BaseAwake();

			fireteamData = ThisContainer.GetData<FireteamData>();
			fireunitGrouping = ThisContainer.GetComponent<FireunitGrouping>();
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
		}
	}
}
