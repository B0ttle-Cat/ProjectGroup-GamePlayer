using System;
using System.Collections.Generic;

using BC.Base;

using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	[Serializable]
	public struct DiplomacyItem : IEquatable<DiplomacyItem>
	{
		[TableColumnWidth(25, Resizable = false)]
		[UnityEngine.SerializeField]
		private bool AI;
		[TableColumnWidth(100, Resizable = false)]
		[UnityEngine.SerializeField]
		private string factionName;
		[TableColumnWidth(100, Resizable = false)]
		[UnityEngine.SerializeField]
		private int factionIndex;
		public List<FriendshipItem> friendshipItems;

		public bool IsAIFaction { get => AI; internal set => AI=value; }
		public string FactionName => factionName;
		public int FactionIndex { get => factionIndex; internal set => factionIndex=value; }
		public bool IsDefault => factionName.IsNullOrWhiteSpace();

		public int GetFriendshipValue(int targetIndex)
		{
			if(FactionIndex == targetIndex)
			{
				return 100;
			}
			if(targetIndex < 0) return 0;

			int Count = friendshipItems.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				FriendshipItem item = friendshipItems[i];
				if(item.factionIndex == targetIndex)
				{
					return item.friendshipValue;
				}
			}
			return 0;
		}
		public void SetFriendshipValue(int targetIndex, int value)
		{
			if(FactionIndex == targetIndex)
			{
				return;
			}
			if(targetIndex < 0) return;


			int Count = friendshipItems.Count;
			for(int i = 0 ; i < Count ; i++)
			{
				var item = friendshipItems[i];

				if(item.factionIndex == targetIndex)
				{
					item.friendshipValue = value;
					friendshipItems[i] = item;
					return;
				}
			}
			friendshipItems.Add(new FriendshipItem()
			{
				factionIndex = targetIndex,
				friendshipValue = value
			});
		}

		public override bool Equals(object obj)
		{
			return obj is DiplomacyItem item && Equals(item);
		}

		public bool Equals(DiplomacyItem other)
		{
			return FactionIndex ==other.FactionIndex;
		}

		public override int GetHashCode()
		{
			return FactionIndex;
		}

		public override string ToString()
		{
			return factionName;
		}

		public static bool operator ==(DiplomacyItem left, DiplomacyItem right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(DiplomacyItem left, DiplomacyItem right)
		{
			return !(left==right);
		}
	}


}
