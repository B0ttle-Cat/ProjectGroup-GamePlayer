using System;
using System.Collections.Generic;

using BC.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "StartMapSetting", menuName = "BC/StartSetting/new StartMapSetting")]
	public partial class StartMapSetting : ScriptableObject
	{
		[SerializeField, HideLabel]
		[FoldoutGroup("PlayMapObject")]
		public ResourcesKey playMapKey;
		[SerializeField, HideLabel]
		[FoldoutGroup("MapStageInfo")]
		public MapStageInfo playMapStageInfo;

		[Serializable]
		public partial struct MapStageInfo
		{
			[Serializable]
			public partial struct AnchorInfo
			{
				public string anchorName;
				public int anchorIndex;
			}
			[TableList]
			public List<AnchorInfo> anchorInfos;
		}
	}
}
