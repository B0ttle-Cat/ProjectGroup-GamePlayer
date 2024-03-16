using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

using static BC.LowLevelAI.NavMeshConnectComputer;

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
			MapAICellData cellIndexData = ThisContainer.GetData<MapAICellData>();

			cellIndexData.trianglesTile = new Dictionary<Vector3Int, List<Triangle>>();
			cellIndexData.trianglesToLink = new Dictionary<Triangle, LinkTriangle[]>();
			//cellIndexData.closeWaypoint = new Dictionary<Triangle, MapWaypoint>();

			DiplomacyQuery = QuerySystemBuilder.CreateQuery().WithAll<DiplomacyData, DiplomacyComputer>().Build();
			OdccQueryCollector.CreateQueryCollector(DiplomacyQuery);
		}
		public override void BaseDestroy()
		{

		}
	}
}
