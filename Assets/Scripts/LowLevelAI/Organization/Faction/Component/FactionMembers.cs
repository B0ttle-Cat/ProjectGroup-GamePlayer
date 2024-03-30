using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FactionMembers : ComponentBehaviour
	{
		private FactionData factionData;
		[SerializeField, ReadOnly]
		private List<FireteamObject> thisMember;

		private OdccQueryCollector  memberCollector;

		public override void BaseAwake()
		{
			base.BaseAwake();

			thisMember = new List<FireteamObject>();

			factionData = ThisContainer.GetData<FactionData>();

			QuerySystem unitQuery = QuerySystemBuilder.CreateQuery()
				.WithAll<FireteamObject, FireteamData, FireteamMembers>()
				.Build();

			memberCollector = OdccQueryCollector.CreateQueryCollector(unitQuery)
				.CreateChangeListEvent(InitList, UpdateList);

		}

		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(memberCollector != null)
			{
				memberCollector.DeleteChangeListEvent(UpdateList);
			}
			memberCollector = null;
			thisMember = null;
		}

		private void InitList(IEnumerable<ObjectBehaviour> enumerable)
		{
			thisMember = enumerable.Select(item => item as FireteamObject)
				.Where(item => item != null && item.ThisContainer.GetData<IFireteamData>().IsEqualsFaction(factionData))
				.ToList();
		}

		private void UpdateList(ObjectBehaviour behaviour, bool isAdded)
		{
			if(behaviour is not FireteamObject unit) return;

			if(isAdded)
			{
				if(!thisMember.Contains(unit) && unit.ThisContainer.GetData<IFireteamData>().IsEqualsFaction(factionData))
				{
					thisMember.Add(unit);
				}
			}
			else
			{
				if(thisMember.Remove(unit))
				{

				}
			}
		}
	}
}
