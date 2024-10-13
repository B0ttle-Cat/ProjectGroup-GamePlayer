using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public enum FactionControlType
	{
		Local = 0,
		Remote = 1,
		AI = 10
	}
	public interface IFactionIndex
	{
		public int FactionIndex { get; set; }
		public bool IsEqualsFaction(int faction)
		{
			return FactionIndex == faction;
		}
		public bool IsEqualsFaction(IFactionIndex factionData)
		{
			return FactionIndex == factionData.FactionIndex;
		}
		public bool IsEqualsFaction(FactionData factionData)
		{
			return FactionIndex == factionData.FactionIndex;
		}
	}
	public interface IFactionData : IFactionIndex, IMemberData
	{

	}
	public class FactionData : MemberData, IFactionData
	{
		[SerializeField, EnumPaging]
		private FactionControlType factionControlType;

		[SerializeField]
		private int factionIndex;

		[SerializeField]
		private string factionName;

		public FactionControlType FactionControlType { get => factionControlType; set => factionControlType = value; }
		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
		public string FactionName { get => factionName; set => factionName = value; }
	}
}
