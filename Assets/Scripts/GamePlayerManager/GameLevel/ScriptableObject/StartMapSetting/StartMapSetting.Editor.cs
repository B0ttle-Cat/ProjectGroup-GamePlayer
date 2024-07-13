#if UNITY_EDITOR
using System.Collections.Generic;

using BC.LowLevelAI;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public partial class StartMapSetting : IConnectStartGameSetting_Editor //.Editor
	{
		[ShowInInspector, ReadOnly, PropertyOrder(-999)]
		public StartGameSetting startGameSetting { get; set; }
		public void ConnectStartGameSetting(StartGameSetting startGameSetting)
		{
			this.startGameSetting = startGameSetting;
		}



		[PropertyOrder(-99)]
		[InlineButton("_UpdateMapStage", "Update MapStage")]
		[PropertySpace(SpaceAfter = 20)]
		[SerializeField]
		[LabelWidth(100)]
		[AssetSelector]
		private MapStage target;

		private void _UpdateMapStage()
		{
			if(target == null) return;
			playMapKey.target = target;
			playMapKey.UpdateSetupTarget();

			var strategicPoints = target.GetComponentsInChildren<StrategicPoint>(true);
			var anchorInfos = new List<MapStageInfo.AnchorInfo>();
			int length = strategicPoints.Length;
			for(int i = 0 ; i < length ; i++)
			{
				var point = strategicPoints[i];
				MapStageInfo.AnchorInfo anchorInfo = new MapStageInfo.AnchorInfo(){
					anchorName = point.gameObject.name,
					anchorIndex = point.gameObject.transform.GetSiblingIndex(),

				};

				anchorInfos.Add(anchorInfo);
			}
			playMapStageInfo = new MapStageInfo();
			playMapStageInfo.anchorInfos = anchorInfos;
		}
	}
}
#endif