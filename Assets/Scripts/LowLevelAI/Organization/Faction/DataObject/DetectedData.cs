using System;
using System.Collections.Generic;

using BC.GameBaseInterface;
using BC.ODCC;

namespace BC.LowLevelAI
{
	public class DetectedData : DataObject
	{
		public List<Info> List = new List<Info>();

		public Info this[int index] { get => List[index]; set => List[index] = value; }
		public int Count => List.Count;

		internal void AddRange(List<Info> calculationList)
		{
			List.AddRange(calculationList);
		}

		internal void Clear()
		{
			List.Clear();
		}

		[Serializable]
		public class Info
		{
			public FireunitDetector detectorComponent;
			public FactionData factionData;
			public FireteamData fireteamData;
			public FactionDiplomacyType factionDiplomacyType;
		}
	}
}
