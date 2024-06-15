using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Cinemachine;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMembers : ComponentBehaviour
	{
		private FireteamData fireteamData;
		[SerializeField, ReadOnly]
		private List<FireunitObject> thisMember;
		private CinemachineTargetGroup cinemachineTargetGroup;

		[SerializeField] private OdccQueryCollector  memberCollector;
		public List<FireunitObject> MemberList => thisMember;
		public int Count => thisMember.Count;


		public Vector3 CenterPosition { get; private set; }
		public override void BaseAwake()
		{
			base.BaseAwake();

			thisMember = new List<FireunitObject>();

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
			thisMember = null;

			cinemachineTargetGroup = null;
		}
		public override void BaseUpdate()
		{
			base.BaseUpdate();
			CenterPosition =_CenterPosition();
		}
		public override void BaseLateUpdate()
		{
			base.BaseLateUpdate();
			CenterPosition =_CenterPosition();
		}
		private void InitList(IEnumerable<ObjectBehaviour> enumerable)
		{
			thisMember = enumerable.Select(item => item as FireunitObject)
				.Where(item => item != null && item.ThisContainer.GetData<IFireunitData>().IsEqualsTeam(fireteamData))
				.ToList();

			UpdateCinemachineTargetGroupMember(thisMember);
		}
		private void UpdateList(ObjectBehaviour behaviour, bool isAdded)
		{
			if(behaviour is not FireunitObject unit) return;

			if(isAdded)
			{
				if(!thisMember.Contains(unit) && unit.ThisContainer.GetData<IFireunitData>().IsEqualsTeam(fireteamData))
				{
					thisMember.Add(unit);
					AddedCinemachineTargetGroupMember(unit);
				}
			}
			else
			{
				if(thisMember.Remove(unit))
				{
					RemoveCinemachineTargetGroupMember(unit);
				}
			}
		}
		public void Foreach(Action<FireunitObject> action, Func<FireunitObject, bool> condition = null)
		{
			if(action == null) return;

			int length = thisMember.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var member = thisMember[i];
				if(condition == null || condition.Invoke(member))
				{
					action.Invoke(member);
				}
			}
		}
		public void Foreach(Action<FireunitObject, int> action, Func<FireunitObject, int, bool> condition = null)
		{
			if(action == null) return;

			int length = thisMember.Count;
			for(int i = 0 ; i < length ; i++)
			{
				var member = thisMember[i];
				if(condition == null || condition.Invoke(member, i))
				{
					action.Invoke(member, i);
				}
			}
		}

		internal void SetCinemachineTargetGroup(CinemachineTargetGroup cinemachineTargetGroup)
		{
			this.cinemachineTargetGroup = cinemachineTargetGroup;
			UpdateCinemachineTargetGroupMember(thisMember);
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

		private Vector3 _CenterPosition()
		{
			Vector3 sumPosition = Vector3.zero;
			int length = thisMember.Count;
			if(length == 0) return sumPosition;
			for(int i = 0 ; i < length ; i++)
			{
				var member = thisMember[i];
				sumPosition += member.ThisTransform.position;
			}
			sumPosition /= length;
			return sumPosition;
		}
	}
}
