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
		public struct FactionSettingInfo
		{
			[SerializeField, EnumPaging]
			private FactionControlType factionControlType;
			[SerializeField]
			private int factionIndex;
			[SerializeField]
			private string factionName;

			public FactionControlType FactionControl { get => factionControlType; set => factionControlType=value; }
			public int FactionIndex { get => factionIndex; set => factionIndex=value; }
			public string FactionName { get => factionName; set => factionName=value; }
		}
		[Serializable]
		public partial struct DiplomacySettingInfo
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
		public List<FactionSettingInfo> factionInfoList;

		[PropertyOrder(2)]
		[ListDrawerSettings(ShowPaging = false),ReadOnly]//, OnBeginListElementGUI = "DrawOnBeginListElementGUI", OnEndListElementGUI = "DrawOnEndListElementGUI")]
		public List<DiplomacySettingInfo> diplomacyInfoList;


		public FactionData ConvertSettingToData()
		{
			return null;
		}

	}
}
