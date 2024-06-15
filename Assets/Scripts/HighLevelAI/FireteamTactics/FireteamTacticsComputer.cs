using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FireteamTacticsComputer : ComponentBehaviour
	{
		[SerializeField]
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

			fireteamTacticsAICollector.CreateLooperEvent(nameof(FireteamTacticsComputer))
				.SetBreakFunction(() => watingStrategyComputing)
				.CallNext(StartAction)
				.Foreach<FireteamTacticsAI, FireteamData, FireteamTacticsData>(StartComputing)
				.Foreach<FireteamTacticsAI, FireteamData, FireteamTacticsData>(UpdateComputing)
				.CallNext(EndedAction);
		}

		public override void BaseDisable()
		{
			base.BaseDisable();

			if(fireteamTacticsAICollector != null)
			{
				fireteamTacticsAICollector.DeleteLooperEvent(nameof(FireteamTacticsComputer));
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

		private void StartComputing(OdccQueryLooper.LoopInfo loopInfo, FireteamTacticsAI tacticsAI, FireteamData fireteamData, FireteamTacticsData tacticsData)
		{
			tacticsAI.UpdateTacticsValue();
		}
		private void UpdateComputing(OdccQueryLooper.LoopInfo loopInfo, FireteamTacticsAI tacticsAI, FireteamData fireteamData, FireteamTacticsData tacticsData)
		{
			tacticsAI.UpdateTacticsState();
		}
	}
}
