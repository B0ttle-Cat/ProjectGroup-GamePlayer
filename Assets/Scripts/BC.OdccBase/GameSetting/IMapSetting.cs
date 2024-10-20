using System;
using System.Collections.Generic;

using BC.Base;

using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	public interface IMapSetting
	{
		public ResourcesKey PlayMapKey { get; }
		public MapStageInfo PlayMapStageInfo { get; }

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
