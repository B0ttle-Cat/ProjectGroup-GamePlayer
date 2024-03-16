using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.LowLevelAI;
using BC.ODCC;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FactionStrategyComputer : ComponentBehaviour
	{
		private FireteamTacticsComputer fireteamTacticsComputer;
		private MapWaypointComputer mapWaypointComputer;

		private OdccQueryCollector factionStrategyAICollector;
		private OdccQueryCollector fireteamStrategyAICollector;
		private OdccQueryCollector strategicPointCollector;

		public float StrategyUpdateTime;
		private bool IsWaitWaypointComputing => mapWaypointComputer == null ? true : mapWaypointComputer.IsComputing;
		private IEnumerable<StrategicPoint> allStrategicPointList { get; set; }

		public override void BaseAwake()
		{
			base.BaseAwake();

			mapWaypointComputer = ThisContainer.GetComponent<MapWaypointComputer>();
			fireteamTacticsComputer = ThisContainer.GetComponent<FireteamTacticsComputer>();

			factionStrategyAICollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery()
				.WithAll<FactionStrategyAI, FactionData, FactionStrategyData>()
				.Build());

			strategicPointCollector = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery()
				.WithAll<MapAnchor, StrategicPoint>()
				.Build());
		}

		public override void BaseEnable()
		{
			base.BaseEnable();

			factionStrategyAICollector.CreateLooper(nameof(FactionStrategyComputer))
				.IsBreakFunction(() => IsWaitWaypointComputing)
				.Action(StartAction)
				.Foreach<FactionStrategyAI>(UpdateComputer)
				.Action(EndedAction)
				.Action(WaitNextUpdate)
				;

			allStrategicPointList = strategicPointCollector.GetQueryItems().Select(item => item.ThisContainer.GetComponent<StrategicPoint>());
		}

		public override void BaseDisable()
		{
			base.BaseDisable();

			if(factionStrategyAICollector != null)
			{
				factionStrategyAICollector.DeleteLooper(nameof(FactionStrategyComputer));
			}
		}


		private void StartAction()
		{
			fireteamTacticsComputer.StartStrategyComputing();
		}
		private void EndedAction()
		{
			fireteamTacticsComputer.EndedStrategyComputing();
		}
		private IEnumerator WaitNextUpdate()
		{
			float nextUpdateTime = StrategyUpdateTime;
			while(nextUpdateTime > 0)
			{
				yield return null;
				nextUpdateTime -= Time.deltaTime;
			}
		}

		private void UpdateComputer(FactionStrategyAI strategyAI)
		{
			strategyAI.UpdateNextStrategicPoint(allStrategicPointList);
			strategyAI.CalculateNextStrategicPoint();
			strategyAI.SelectNextStrategicPath();
			strategyAI.GiveOrderToFireteam();
		}
	}
}
