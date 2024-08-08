using System.Collections;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IFireunitData : IMemberData, IFireteamData
	{
		public int MemberUniqueID { get; }
		public int UnitIndex { get; set; }
		public bool IsEqualsUnit(int faction, int team, int unit)
		{
			return IsEqualsTeam(faction, team) && UnitIndex == unit;
		}
		public bool IsEqualsUnit(IFireunitData fireunitData)
		{
			return IsEqualsTeam(fireunitData) && UnitIndex == fireunitData.UnitIndex;
		}
		public bool IsEqualsUnit(FireunitData fireunitData)
		{
			return IsEqualsTeam(fireunitData) && UnitIndex == fireunitData.UnitIndex;
		}
		public void SetData(int faction, int team, int unit)
		{
			FactionIndex = faction;
			TeamIndex = team;
			UnitIndex = unit;
		}
	}
	public class FireunitData : MemberData, IFireunitData
	{
		[SerializeField]
		[ValueDropdown("ShowTargetFactionName")]
		private int factionIndex;
		[SerializeField]
		private int fireteamIndex;
		[SerializeField]
		private int fireunitIndex;

		public int MemberUniqueID { get => 1000000 + (FactionIndex * 010000) + (TeamIndex * 000100) + (UnitIndex); }
		public int FactionIndex { get => factionIndex; set => factionIndex = value; }
		public int TeamIndex { get => fireteamIndex; set => fireteamIndex = value; }
		public int UnitIndex { get => fireunitIndex; set => fireunitIndex = value; }

#if UNITY_EDITOR
		public static IEnumerable ShowTargetFactionName()
		{
			if(OdccContainerTree.TryFindOdccType_InEditor<FactionData>(out var result))
			{

			}

			return null;
		}
#endif
	}
}
