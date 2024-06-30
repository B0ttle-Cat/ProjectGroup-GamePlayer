using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public class FactionDiplomacyData : DataObject
	{
		private Dictionary<int, FactionDiplomacyType> factionDiplomacy;

		public Dictionary<int, FactionDiplomacyType> FactionDiplomacy { get => factionDiplomacy; set => factionDiplomacy=value; }
	}
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
