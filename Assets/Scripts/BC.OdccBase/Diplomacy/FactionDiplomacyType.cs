using UnityEngine;

namespace BC.OdccBase
{
	public enum FactionDiplomacyType : int
	{
		[InspectorName("Enemy")]
		Enemy_Faction = -10,        // 적대적 세력
		[InspectorName("Neutral")]
		Neutral_Faction = 0,        // 중립적 세력
		[InspectorName("Alliance")]
		Alliance_Faction = 10,      // 동맹 세력
		[InspectorName("Equal")]
		Equal_Faction = 100,        // 동일 세력
	}
}
