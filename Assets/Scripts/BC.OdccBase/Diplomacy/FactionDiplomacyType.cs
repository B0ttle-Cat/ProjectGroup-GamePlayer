using UnityEngine;

namespace BC.OdccBase
{
	public enum FactionDiplomacyType : int
	{
		[InspectorName("Enemy")]
		Enemy_Faction = -10,        // ������ ����
		[InspectorName("Neutral")]
		Neutral_Faction = 0,        // �߸��� ����
		[InspectorName("Alliance")]
		Alliance_Faction = 10,      // ���� ����
		[InspectorName("Equal")]
		Equal_Faction = 100,        // ���� ����
	}
}
