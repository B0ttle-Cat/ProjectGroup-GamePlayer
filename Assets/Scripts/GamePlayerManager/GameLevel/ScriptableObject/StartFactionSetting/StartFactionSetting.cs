using System;
using System.Collections.Generic;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{

	[CreateAssetMenu(fileName = "StartFactionSetting", menuName = "BC/StartSetting/new StartFactionSetting")]
	public partial class StartFactionSetting : ScriptableObject
	{
		[Serializable]
		public struct FactionInfo
		{
			[SerializeField]
			private int factionIndex;
			[SerializeField]
			private string factionName;

			public int FactionIndex { get => factionIndex; set => factionIndex=value; }
			public string FactionName { get => factionName; set => factionName=value; }
		}
		[Serializable]
		public partial struct DiplomacyItem
		{
			[ValueDropdown("ShowFactionList")]
			[SerializeField, ReadOnly]
			[HideLabel, HorizontalGroup(width:100)]
			private int factionActor;

			[ValueDropdown("ShowFactionList")]
			[SerializeField, ReadOnly]
			[HideLabel, HorizontalGroup(width:100)]
			private int factionTarget;

			[SerializeField, EnumToggleButtons, DisableIf("IsEqualFaction")]
			[HideLabel, HorizontalGroup]
			private FactionDiplomacyType factionDiplomacy;

			public int FactionActor { get => factionActor; set => factionActor=value; }
			public int FactionTarget { get => factionTarget; set => factionTarget=value; }
			public FactionDiplomacyType FactionDiplomacy { get => factionDiplomacy; set => factionDiplomacy=value; }
		}

		[TableList]
		public List<FactionInfo> factionInfoList;

		//[TableList]
		[PropertyOrder(2)]
		[ListDrawerSettings(ShowPaging = false, OnBeginListElementGUI = "DrawOnBeginListElementGUI", OnEndListElementGUI = "DrawOnEndListElementGUI")]
		public List<DiplomacyItem> DiplomacyList;
	}
}
