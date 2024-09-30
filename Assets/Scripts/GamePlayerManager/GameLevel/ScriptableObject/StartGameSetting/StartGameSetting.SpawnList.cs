using System;
using System.Collections;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public partial class StartGameSetting//.SpawnList
	{
		[Serializable]
		public partial class SpawnAnchor : IFireteamData
		{
			[ValueDropdown("ShowFactionList")]
			[TableColumnWidth(150, false)]
			[SerializeField, ReadOnly]
			private int factionIndex;
			[ValueDropdown("ShowTeamList")]
			[TableColumnWidth(100, false)]
			[SerializeField, ReadOnly]
			private int teamIndex;
			[ValueDropdown("ShowAnchorList")]
			[SerializeField]
			private int anchorIndex;

			public int FactionIndex { get => factionIndex; set => factionIndex=value; }
			public int TeamIndex { get => teamIndex; set => teamIndex=value; }
			public int AnchorIndex { get => anchorIndex; set => anchorIndex=value; }

			public void Dispose()
			{
			}
		}

#if UNITY_EDITOR
		[PropertyOrder(-1)]
		[TabGroup("Tap", nameof(SpawnList))]
		[Button("Fill Spawn Anchor List")]
		public void FillSpawnAnchorList()
		{
			if(unitSetting == null) return;
			SpawnList ??= new System.Collections.Generic.List<SpawnAnchor>();
			var list = SpawnList;
			foreach(var character in unitSetting.characterDatas)
			{
				int findIndex = list.FindIndex(item =>
				item.FactionIndex == character.FactionIndex &&
				item.TeamIndex == character.TeamIndex);

				if(findIndex < 0)
				{
					list.Add(new() {
						FactionIndex = character.FactionIndex,
						TeamIndex = character.TeamIndex,
						AnchorIndex = -1,
					});
				}
			}
			SortCharacterDatas();

			void SortCharacterDatas()
			{
				if(list != null && list.Count > 0)
				{
					list.Sort((a, b) => {
						int compare = a.FactionIndex.CompareTo(b.FactionIndex);
						if(compare != 0) return compare;
						compare = a.TeamIndex.CompareTo(b.TeamIndex);
						return compare;
					});
				}
			}
		}

		public partial class SpawnAnchor : IShowFactionList, IShowTeamList, IShowUnitList, IShowAnchorList
		{
			public IEnumerable ShowFactionList()
			{
				return (this as IShowFactionList)._ShowFactionList();
			}
			public IEnumerable ShowTeamList()
			{
				return (this as IShowTeamList)._ShowTeamList();
			}
			public IEnumerable ShowUnitList()
			{
				return (this as IShowUnitList)._ShowUnitList();
			}

			public IEnumerable ShowAnchorList()
			{
				return (this as IShowAnchorList)._ShowAnchorList();
			}
		}
#endif
	}
}
