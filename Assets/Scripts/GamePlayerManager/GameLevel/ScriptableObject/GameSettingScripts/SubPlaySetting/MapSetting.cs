using System;
using System.Collections.Generic;

using BC.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class MapSetting : SubPlaySetting
	{
		[SerializeField, HideLabel]
		[Title("ResourcesKey")]
		public ResourcesKey playMapKey;

		[Title("MapStageInfo")]
		[SerializeField, HideLabel]
		public MapStageInfo playMapStageInfo;

		[Serializable]
		public partial struct MapStageInfo
		{
			[Serializable]
			public partial struct AnchorInfo
			{
				[HorizontalGroup("ID"), HideLabel, SuffixLabel("Index", Overlay = true)]
				public int anchorIndex;
				[HorizontalGroup("ID"), HideLabel, SuffixLabel("Name", Overlay = true)]
				public string anchorName;
			}

			public List<AnchorInfo> anchorInfos;
		}
	}
}
