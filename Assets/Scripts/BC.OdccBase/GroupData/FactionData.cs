using System.Collections;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IFactionData : IOdccData
	{
		public int FactionIndex { get; set; }
		public bool IsEqualsFaction(int faction)
		{
			return FactionIndex == faction;
		}
		public bool IsEqualsFaction(IFactionData factionData)
		{
			return FactionIndex == factionData.FactionIndex;
		}
		public bool IsEqualsFaction(FactionData factionData)
		{
			return FactionIndex == factionData.FactionIndex;
		}
	}
	public class FactionData : DataObject, IFactionData
	{
		[SerializeField]
		[ValueDropdown("ShowTargetFactionName")]
		private int factionIndex;

		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
#if UNITY_EDITOR
		public static IEnumerable ShowTargetFactionName()
		{
			return null; // FriendshipItem.ShowTargetFactionName();
		}
#endif
	}
}
