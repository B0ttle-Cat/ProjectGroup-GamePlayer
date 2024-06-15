using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Cinemachine;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireteamMemberCollector : MemberCollectorComponent<FireunitObject>
	{
		private FireteamData fireteamData;
		[SerializeField, ReadOnly]
		private CinemachineTargetGroup cinemachineTargetGroup;

		public Vector3 CenterPosition { get; private set; } = Vector3.zero;
		public Vector3 Direction { get; private set; } = Vector3.forward;
		public override void BaseAwake_MemberCollector(ref QuerySystem memberQuery)
		{
			fireteamData = ThisContainer.GetData<FireteamData>();

			memberQuery = QuerySystemBuilder.CreateQuery()
				.WithAll<FireunitObject, FireunitData, FireunitController>()
				.Build();

			cinemachineTargetGroup = cinemachineTargetGroup != null ? cinemachineTargetGroup : GetComponent<CinemachineTargetGroup>();
		}
		public override void BaseDestroy_MemberCollector()
		{
			fireteamData = null;
			cinemachineTargetGroup = null;
		}
		public override List<FireunitObject> Base_MemberInitList(IEnumerable<FireunitObject> enumerable)
		{
			return enumerable.Where(item => item != null && item.ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsTeam(fireteamData)).ToList();
		}
		public override void Base_MemberUpdateList(FireunitObject member, bool isAdded)
		{
			if(isAdded)
			{
				if(!ThisMembers.Contains(member) && member.ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsTeam(fireteamData))
				{
					ThisMembers.Add(member);
					AddedCinemachineTargetGroupMember(member);
				}
			}
			else
			{
				if(ThisMembers.Remove(member))
				{
					RemoveCinemachineTargetGroupMember(member);
				}
			}
		}

		public override void BaseUpdate()
		{
			base.BaseUpdate();
			Vector3 oldCenter = CenterPosition;
			CenterPosition =_CenterPosition();

			if(Vector3.Distance(oldCenter, CenterPosition) > 0.001f)
			{
				Direction = (oldCenter - CenterPosition).normalized;
			}
		}
		public override void BaseLateUpdate()
		{
			base.BaseLateUpdate();
		}

		internal void SetCinemachineTargetGroup(CinemachineTargetGroup cinemachineTargetGroup)
		{
			this.cinemachineTargetGroup = cinemachineTargetGroup;
			UpdateCinemachineTargetGroupMember(ThisMembers);
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

			int length = Count;
			if(length == 0) return sumPosition;

			Foreach(member => sumPosition+=member.ThisTransform.position);

			sumPosition /= length;
			return sumPosition;
		}

		public bool TryFindFireunit(int fireunitIndex, out FireunitObject fireunitObject)
		{
			fireunitObject = null;
			int count = Count;
			for(int i = 0 ; i < count ; i++)
			{
				if(ThisMembers[i].ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsUnit(fireteamData.FactionIndex, fireteamData.TeamIndex, fireunitIndex))
				{
					fireunitObject = ThisMembers[i];
					break;
				}
			}
			return fireunitObject != null;
		}
	}
}
