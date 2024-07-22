using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class RootMemberCollector : MemberCollectorComponent<FactionObject>, IFindCollectedMembers
	{
		public override void BaseAwake_MemberCollector(ref QuerySystem memberCollector)
		{
			memberCollector = QuerySystemBuilder.CreateQuery()
				.WithAll<FactionObject, FactionData>()
				.Build();
		}

		public override void BaseDestroy_MemberCollector()
		{
		}

		public override List<FactionObject> Base_MemberInitList(IEnumerable<FactionObject> enumerable)
		{
			return enumerable.Where(item => item != null).ToList();
		}

		public override void Base_MemberUpdateList(FactionObject member, bool isAdded)
		{
			if(isAdded)
			{
				if(!ThisMembers.Contains(member))
				{
					ThisMembers.Add(member);
				}
			}
			else
			{
				if(ThisMembers.Remove(member))
				{

				}
			}
		}

		public bool TryFindFaction(int factionIndex, out ObjectBehaviour factionObject)
		{
			factionObject = ThisMembers.Find(item => item.ThisContainer.TryGetData<FactionData>(out var data) && data.FactionIndex == factionIndex);
			return factionObject != null;
		}

		public bool TryFindFireteam(int factionIndex, int fireteamIndex, out ObjectBehaviour fireteamObject)
		{
			fireteamObject = null;

			if(!TryFindFaction(factionIndex, out var factionObject)) return false;
			if(!factionObject.ThisContainer.TryGetComponent<FactionMemberCollector>(out var findMember)) return false;
			if(!findMember.TryFindFireteam(fireteamIndex, out var findObject)) return false;

			fireteamObject = findObject;
			return factionObject != null;
		}

		public bool TryFindFireunit(int factionIndex, int fireteamIndex, int fireunitIndex, out ObjectBehaviour fireunitObject)
		{
			fireunitObject = null;

			if(!TryFindFireteam(factionIndex, fireteamIndex, out var teamObject)) return false;
			if(!teamObject.ThisContainer.TryGetComponent<FireteamMemberCollector>(out var findMember)) return false;
			if(!findMember.TryFindFireunit(fireunitIndex, out var findObject)) return false;

			fireunitObject = findObject;
			return fireunitObject != null;
		}
	}
}
