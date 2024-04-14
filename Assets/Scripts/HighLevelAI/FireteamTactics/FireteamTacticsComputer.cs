using BC.GameBaseInterface;
using BC.ODCC;

namespace BC.HighLevelAI
{
	public class FireteamTacticsComputer : ComponentBehaviour
	{
		private OdccQueryCollector fireteamTacticsAICollector;
		private bool watingStrategyComputing;

		public override void BaseAwake()
		{
			base.BaseAwake();
			fireteamTacticsAICollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery()
				.WithAll<FireteamTacticsAI, FireteamData, FireteamTacticsData>()
				.Build());
		}

		public override void BaseEnable()
		{
			base.BaseEnable();

			fireteamTacticsAICollector.CreateLooper(nameof(FireteamTacticsComputer))
				.IsBreakFunction(() => watingStrategyComputing)
				.Action(StartAction)
				.Foreach<FireteamTacticsAI, FireteamData, FireteamTacticsData>(StartComputing)
				.Foreach<FireteamTacticsAI, FireteamData, FireteamTacticsData>(UpdateComputing)
				.Action(EndedAction);
		}

		public override void BaseDisable()
		{
			base.BaseDisable();

			if(fireteamTacticsAICollector != null)
			{
				fireteamTacticsAICollector.DeleteLooper(nameof(FireteamTacticsComputer));
			}
		}

		internal void StartStrategyComputing()
		{
			watingStrategyComputing = true;
		}
		internal void EndedStrategyComputing()
		{
			watingStrategyComputing = false;
		}
		private void EndedAction()
		{

		}

		private void StartAction()
		{

		}

		private void StartComputing(FireteamTacticsAI tacticsAI, FireteamData fireteamData, FireteamTacticsData tacticsData)
		{
			tacticsAI.UpdateTacticsValue();
		}
		private void UpdateComputing(FireteamTacticsAI tacticsAI, FireteamData fireteamData, FireteamTacticsData tacticsData)
		{
			tacticsAI.UpdateTacticsState();
		}
	}
}
