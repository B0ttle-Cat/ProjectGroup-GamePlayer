using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Cinemachine;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamController : ComponentBehaviour
	{
		private FireteamData fireteamData;
		private FireunitGrouping fireunitGrouping;
		[SerializeField]
		private CinemachineTargetGroup cinemachineTargetGroup;

		private OdccQueryCollector findGrouping;
		public override void BaseAwake()
		{
			base.BaseAwake();

			fireteamData = ThisContainer.GetData<FireteamData>();
			cinemachineTargetGroup = cinemachineTargetGroup != null ? cinemachineTargetGroup : GetComponent<CinemachineTargetGroup>();

			var findQuery = QuerySystemBuilder.CreateQuery().WithAll<FireunitGrouping, FireteamData>().Build();
			findGrouping = OdccQueryCollector.CreateQueryCollector(findQuery).CreateChangeListEvent(InitList, UpdateList);
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();

			if(findGrouping != null)
			{
				findGrouping.DeleteChangeListEvent(UpdateList);
			}
			findGrouping = null;
			fireunitGrouping = null;
		}

		private void InitList(IEnumerable<ObjectBehaviour> enumerable)
		{
			var grouping = enumerable.Select(item => item as FireunitGrouping)
				.FirstOrDefault(item => item != null && item.ThisContainer.GetData<IFireteamData>().IsEqualsTeam(fireteamData));

			if(grouping != null)
			{
				SetFireunitGrouping(grouping);
			}
		}

		private void UpdateList(ObjectBehaviour behaviour, bool isAdded)
		{
			if(isAdded)
			{
				if(behaviour is FireunitGrouping grouping)
				{
					if(grouping.ThisContainer.GetData<IFireteamData>().IsEqualsTeam(fireteamData))
					{
						SetFireunitGrouping(grouping);
					}
				}
			}
			else
			{
				if(behaviour is FireunitGrouping grouping && fireunitGrouping == grouping)
				{
					SetFireunitGrouping(null);
				}
			}
		}

		private void SetFireunitGrouping(FireunitGrouping grouping)
		{
			fireunitGrouping = grouping;
			if(fireunitGrouping != null)
			{
				fireunitGrouping.SetCinemachineTargetGroup(cinemachineTargetGroup);
			}
		}
	}
}
