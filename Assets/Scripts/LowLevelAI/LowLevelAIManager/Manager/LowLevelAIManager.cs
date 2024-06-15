using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public interface IGetLowLevelAIManager : IOdccObject
	{
		LowLevelAIManager LowLevelAI { get; }
		MapStage MapStage { get { return LowLevelAI.MapStage; } }
	}

	public class LowLevelAIManager : ObjectBehaviour
	{
		public MapStage MapStage { get; private set; }
		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		public static QuerySystem DiplomacyQuery;
		public override void BaseAwake()
		{
			DiplomacyQuery = QuerySystemBuilder.CreateQuery().WithAll<DiplomacyData, DiplomacyComputer>().Build();
			OdccQueryCollector.CreateQueryCollector(DiplomacyQuery);

			MapStage = ThisContainer.GetChildObject<MapStage>();
		}
		public override void BaseDestroy()
		{

		}
	}
}
