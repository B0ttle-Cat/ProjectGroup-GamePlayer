using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public enum FactionDiplomacyType : int
	{
		[InspectorName("적군")]
		Enemy_Faction = -10,        // 적대적 세력
		[InspectorName("중립")]
		Neutral_Faction = 0,        // 중립적 세력
		[InspectorName("아군")]
		Alliance_Faction = 10,      // 동맹 세력
	}

	public class DiplomacyData : DataObject
	{
		public Dictionary<(int,int), FactionDiplomacyType> diplomacyTypeList;
	}
}
