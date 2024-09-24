using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FactionStrategyComputer : ComponentBehaviour
	{
		private FireteamTacticsComputer fireteamTacticsComputer;
		private MapPathPointComputer mapPathPointComputer;

		[SerializeField] private OdccQueryCollector factionStrategyAICollector;
		[SerializeField] private OdccQueryCollector fireteamStrategyAICollector;
		[SerializeField] private OdccQueryCollector strategicPointCollector;

		public float StrategyUpdateTime;
		private bool IsWaitPathPointComputing => mapPathPointComputer == null ? true : mapPathPointComputer.IsComputing;
		private IEnumerable<StrategicPoint> allStrategicPointList { get; set; }

		public override void BaseAwake()
		{
			base.BaseAwake();

			mapPathPointComputer = ThisContainer.GetComponent<MapPathPointComputer>();
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


			allStrategicPointList = strategicPointCollector.GetQueryItems().Select(item => item.ThisContainer.GetComponent<StrategicPoint>());
		}

		public override void BaseDisable()
		{
			base.BaseDisable();

			if(factionStrategyAICollector != null)
			{
				factionStrategyAICollector.DeleteLooperEvent(nameof(FactionStrategyComputer));
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

		//private void UpdateComputer(FactionStrategyAI strategyAI)
		//{
		//	strategyAI.UpdateNextStrategicPoint(allStrategicPointList);
		//	strategyAI.CalculateNextStrategicPoint();
		//	strategyAI.SelectNextStrategicPath();
		//	strategyAI.GiveOrderToFireteam();
		//}
	}
}
