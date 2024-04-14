using System;
using System.Collections;
using System.Linq;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GameBaseInterface
{
	[Serializable]
	public struct FriendshipItem
	{
		[HorizontalGroup("Friendship"),HideLabel]
		[ValueDropdown("ShowTargetFactionName")]
		public int factionIndex;
		[HorizontalGroup("Friendship"),HideLabel]
		[Range(-150,150)]
		public int friendshipValue;

#if UNITY_EDITOR
		public static IEnumerable ShowTargetFactionName()
		{
			var list = DiplomacyTable.SelectDiplomacyTable == null
				? new System.Collections.Generic.List<(string,int)>()
				: DiplomacyTable.SelectDiplomacyTable.ItemList.Select(select => (select.FactionName, select.FactionIndex));


			var result = new ValueDropdownList<int>();
			foreach(var item in list)
			{
				string fieldName = item.FactionName.Replace("\n", " ").Replace("\r", " ");
				result.Add(fieldName, item.FactionIndex);
			}
			return result;
		}
#endif
	}
}
