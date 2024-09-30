using System.Collections;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IFireunitData : IMemberData, IFireteamData
	{
		public Vector3Int MemberUniqueID { get; }
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
		private Vector3Int fireunitData;

		public Vector3Int MemberUniqueID => fireunitData;
		[ShowInInspector, ValueDropdown("ShowTargetFactionName")]
		public int FactionIndex { get => fireunitData.x; set => fireunitData.x = value; }
		[ShowInInspector]
		public int TeamIndex { get => fireunitData.y; set => fireunitData.y = value; }
		[ShowInInspector]
		public int UnitIndex { get => fireunitData.z; set => fireunitData.z = value; }

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
