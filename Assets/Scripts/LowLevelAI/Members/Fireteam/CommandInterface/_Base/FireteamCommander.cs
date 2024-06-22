using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamCommander : EventCommander
	{
		protected override bool SubBaseAddValidation(ComponentBehaviour behaviour)
		{
			return behaviour is FireteamCommandActor;
		}
		protected override bool SubBaseAddValidation(DataObject data)
		{
			return data is FireteamCommandData;
		}
	}
}
