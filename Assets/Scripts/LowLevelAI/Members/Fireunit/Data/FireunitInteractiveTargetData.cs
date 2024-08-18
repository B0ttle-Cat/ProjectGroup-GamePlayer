using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class FireunitInteractiveTargetData : DataObject
	{
		private HashSet<int> hashTargetIDList;
		public UnitInteractiveInfo[] AllTargetList = new UnitInteractiveInfo[0];

		public UnitInteractiveInfo[] EnemyTargetList = new UnitInteractiveInfo[0];
		public UnitInteractiveInfo[] NeutralTargetList = new UnitInteractiveInfo[0];
		public UnitInteractiveInfo[] AllianceTargetList = new UnitInteractiveInfo[0];
		public UnitInteractiveInfo[] EqualTargetList = new UnitInteractiveInfo[0];

		public void Clear()
		{
			hashTargetIDList = new HashSet<int>();
			AllTargetList = new UnitInteractiveInfo[0];

			EnemyTargetList = new UnitInteractiveInfo[0];
			NeutralTargetList = new UnitInteractiveInfo[0];
			AllianceTargetList = new UnitInteractiveInfo[0];
			EqualTargetList = new UnitInteractiveInfo[0];
		}
		public void UpdateList(List<UnitInteractiveInfo> updateTargetList)
		{
			SortBy(ref updateTargetList);

			HashSet<int> existingIds = new HashSet<int>();
			foreach(var item in updateTargetList)
			{
				existingIds.Add(item.Target.UnitData.MemberUniqueID);
			}
			if(hashTargetIDList == null || hashTargetIDList.Count == 0)
			{
				hashTargetIDList = existingIds;
				if(existingIds.Count == 0) return;
			}
			else
			{
				if(existingIds.Count == hashTargetIDList.Count &&
					existingIds.Equals(hashTargetIDList))
				{
					return;
				}
				else
				{
					hashTargetIDList = existingIds;
				}
			}

			AllTargetList = updateTargetList.ToArray();
			List<UnitInteractiveInfo> enemyTargets = new List<UnitInteractiveInfo>();
			List<UnitInteractiveInfo> neutralTargets = new List<UnitInteractiveInfo>();
			List<UnitInteractiveInfo> allianceTargets = new List<UnitInteractiveInfo>();
			List<UnitInteractiveInfo> equalTargets = new List<UnitInteractiveInfo>();

			int Length = AllTargetList.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				UnitInteractiveInfo info = AllTargetList[i];
				switch(info.DiplomacyType)
				{
					case FactionDiplomacyType.Enemy_Faction:
						enemyTargets.Add(info);
						break;
					case FactionDiplomacyType.Neutral_Faction:
						neutralTargets.Add(info);
						break;
					case FactionDiplomacyType.Alliance_Faction:
						allianceTargets.Add(info);
						break;
					case FactionDiplomacyType.Equal_Faction:
						equalTargets.Add(info);
						break;
					default:
						neutralTargets.Add(info);
						break;
				}
			}

			EnemyTargetList = enemyTargets.ToArray();
			NeutralTargetList = neutralTargets.ToArray();
			AllianceTargetList = allianceTargets.ToArray();
			EqualTargetList = equalTargets.ToArray();
		}

		public virtual void SortBy(ref List<UnitInteractiveInfo> updateTargetList)
		{
			updateTargetList.Sort((a, b) => a.Distance.CompareTo(b.Distance));
		}

		protected override void Disposing()
		{
			hashTargetIDList = null;
			AllTargetList = null;

			EnemyTargetList = null;
			NeutralTargetList = null;
			AllianceTargetList = null;
			EqualTargetList = null;
		}
	}
}
