using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public enum FactionDiplomacyType : int
	{
		[InspectorName("����")]
		Enemy_Faction = -10,        // ������ ����
		[InspectorName("�߸�")]
		Neutral_Faction = 0,        // �߸��� ����
		[InspectorName("�Ʊ�")]
		Alliance_Faction = 10,      // ���� ����
	}

	public class DiplomacyData : DataObject
	{
		public Dictionary<(int,int), FactionDiplomacyType> diplomacyTypeList;
	}
}
