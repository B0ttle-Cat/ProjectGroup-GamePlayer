using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IFindCollectedMembers : IOdccComponent
	{
		public bool TryFindFaction(int factionIndex, out ObjectBehaviour factionObject);
		public bool TryFindFireteam(int factionIndex, int fireteamIndex, out ObjectBehaviour fireteamObject);
		public bool TryFindFireunit(int factionIndex, int fireteamIndex, int fireunitIndex, out ObjectBehaviour fireunitObject);
		public bool TryFindFireunit(int factionIndex, int fireteamIndex, int fireunitIndex, out IUnitInteractiveValue fireunitValue);


		public bool TryFindFaction(IFactionData memberData, out ObjectBehaviour factionObject)
		{
			return TryFindFaction(memberData.FactionIndex, out factionObject);
		}
		public bool TryFindFaction(IFireteamData memberData, out ObjectBehaviour factionObject)
		{
			return TryFindFaction(memberData.FactionIndex, out factionObject);
		}
		public bool TryFindFaction(IFireunitData memberData, out ObjectBehaviour factionObject)
		{
			return TryFindFaction(memberData.FactionIndex, out factionObject);
		}

		public bool TryFindFireteam(IFireteamData memberData, out ObjectBehaviour fireteamObject)
		{
			return TryFindFireteam(memberData.FactionIndex, memberData.TeamIndex, out fireteamObject);
		}
		public bool TryFindFireteam(IFireunitData memberData, out ObjectBehaviour fireteamObject)
		{
			return TryFindFireteam(memberData.FactionIndex, memberData.TeamIndex, out fireteamObject);
		}

		public bool TryFindFireunit(IFireunitData memberData, out ObjectBehaviour fireunitObject)
		{
			return TryFindFireunit(memberData.FactionIndex, memberData.TeamIndex, memberData.UnitIndex, out fireunitObject);
		}
		public bool TryFindFireunit(IFireunitData memberData, out IUnitInteractiveValue fireunitValue)
		{
			return TryFindFireunit(memberData.FactionIndex, memberData.TeamIndex, memberData.UnitIndex, out fireunitValue);
		}
		public bool TryFindFireunit(Vector3Int memberUniqueID, out ObjectBehaviour fireunitObject)
		{
			return TryFindFireunit(memberUniqueID.x, memberUniqueID.y, memberUniqueID.z, out fireunitObject);
		}
		public bool TryFindFireunit(Vector3Int memberUniqueID, out IUnitInteractiveValue fireunitValue)
		{
			return TryFindFireunit(memberUniqueID.x, memberUniqueID.y, memberUniqueID.z, out fireunitValue);
		}
	}
}
