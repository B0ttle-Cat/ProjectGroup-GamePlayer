using System;

using BC.ODCC;

namespace BC.Character
{
	[Serializable]
	public class PlayingID : DataObject
	{
		public int playerID;
		public int groupID;
		public int unitID;
	}
}
