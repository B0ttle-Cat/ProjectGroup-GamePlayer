using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Cinemachine;

namespace BC.LowLevelAI
{
	public class FireunitGrouping : ComponentBehaviour
	{
		private FireteamData fireteamData;
		private List<FireunitObject> thisGroupMember;
		private CinemachineTargetGroup cinemachineTargetGroup;

		private OdccQueryCollector  memberCollector;

		public override void BaseAwake()
		{
			base.BaseAwake();

			thisGroupMember = new List<FireunitObject>();

			fireteamData = ThisContainer.GetData<FireteamData>();

			QuerySystem unitQuery = QuerySystemBuilder.CreateQuery()
				.WithAll<FireunitObject, FireunitData, FireunitController>()
				.Build();

			memberCollector = OdccQueryCollector.CreateQueryCollector(unitQuery)
				.CreateChangeListEvent(InitList, UpdateList);

			cinemachineTargetGroup = cinemachineTargetGroup != null ? cinemachineTargetGroup : GetComponent<CinemachineTargetGroup>();
		}

		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(memberCollector != null)
			{
				memberCollector.DeleteChangeListEvent(UpdateList);
			}
			memberCollector = null;
			thisGroupMember = null;

			cinemachineTargetGroup = null;
		}

		private void InitList(IEnumerable<ObjectBehaviour> enumerable)
		{
			thisGroupMember = enumerable.Select(item => item as FireunitObject)
				.Where(item => item != null && item.ThisContainer.GetData<IFireunitData>().IsEqualsTeam(fireteamData))
				.ToList();

			UpdateCinemachineTargetGroupMember(thisGroupMember);
		}

		private void UpdateList(ObjectBehaviour behaviour, bool isAdded)
		{
			if(behaviour is not FireunitObject unit) return;

			if(isAdded)
			{
				if(!thisGroupMember.Contains(unit) && unit.ThisContainer.GetData<IFireunitData>().IsEqualsTeam(fireteamData))
				{
					thisGroupMember.Add(unit);
					AddedCinemachineTargetGroupMember(unit);
				}
			}
			else
			{
				if(thisGroupMember.Remove(unit))
				{
					RemoveCinemachineTargetGroupMember(unit);
				}
			}
		}

		internal void SetCinemachineTargetGroup(CinemachineTargetGroup cinemachineTargetGroup)
		{
			this.cinemachineTargetGroup = cinemachineTargetGroup;
			UpdateCinemachineTargetGroupMember(thisGroupMember);
		}

		private void AddedCinemachineTargetGroupMember(FireunitObject fireunit)
		{
			if(fireunit == null) return;
			if(cinemachineTargetGroup == null) return;

			cinemachineTargetGroup.AddMember(fireunit.ThisTransform, 1f, 1f);
		}
		private void RemoveCinemachineTargetGroupMember(FireunitObject fireunit)
		{
			if(fireunit == null) return;
			if(cinemachineTargetGroup == null) return;

			cinemachineTargetGroup.RemoveMember(fireunit.ThisTransform);
		}
		private void UpdateCinemachineTargetGroupMember(List<FireunitObject> unitList)
		{
			if(unitList == null) return;
			if(cinemachineTargetGroup == null) return;

			cinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
			int Count = unitList.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				cinemachineTargetGroup.AddMember(unitList[i].ThisTransform, 1f, 1f);
			}
		}
	}
}
