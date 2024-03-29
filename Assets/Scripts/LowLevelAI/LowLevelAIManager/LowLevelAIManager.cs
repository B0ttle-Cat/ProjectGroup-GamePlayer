using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface ILowLevelAIManager : IOdccObject
	{
		LowLevelAIManager LowLevelAIManager { get; }
	}

	public class LowLevelAIManager : ObjectBehaviour
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		public static QuerySystem DiplomacyQuery;
		public override void BaseAwake()
		{
			DiplomacyQuery = QuerySystemBuilder.CreateQuery().WithAll<DiplomacyData, DiplomacyComputer>().Build();
			OdccQueryCollector.CreateQueryCollector(DiplomacyQuery);
		}
		public override void BaseDestroy()
		{

		}
	}
}
