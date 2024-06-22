using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class FactionMemberCollector : MemberCollectorComponent<FireteamObject>
	{
		private FactionData factionData;

		public override void BaseAwake_MemberCollector(ref QuerySystem memberQuery)
		{
			factionData = ThisContainer.GetData<FactionData>();

			memberQuery = QuerySystemBuilder.CreateQuery()
				.WithAll<FireteamObject, FireteamData, FireteamMemberCollector>()
				.Build();
		}
		public override void BaseDestroy_MemberCollector()
		{
			factionData = null;
		}

		public override List<FireteamObject> Base_MemberInitList(IEnumerable<FireteamObject> enumerable)
		{
			return enumerable.Where(item => item != null && item.ThisContainer.GetData<IFireteamData>().IsEqualsFaction(factionData)).ToList();
		}
		public override void Base_MemberUpdateList(FireteamObject member, bool isAdded)
		{
			if(isAdded)
			{
				if(!ThisMembers.Contains(member) && member.ThisContainer.TryGetData<IFireteamData>(out var data) && data.IsEqualsFaction(factionData))
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


		public bool TryFindFireteam(int fireteamIndex, out FireteamObject fireteamObject)
		{
			fireteamObject = null;
			int count = Count;
			for(int i = 0 ; i < count ; i++)
			{
				if(ThisMembers[i].ThisContainer.TryGetData<IFireteamData>(out var data) && data.IsEqualsTeam(factionData.FactionIndex, fireteamIndex))
				{
					fireteamObject = ThisMembers[i];
					break;
				}
			}
			return fireteamObject != null;
		}
	}
}
