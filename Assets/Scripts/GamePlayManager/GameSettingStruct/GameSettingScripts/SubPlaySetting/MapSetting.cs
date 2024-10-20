using BC.Base;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class MapSetting : SubPlaySetting, IMapSetting
	{
		[SerializeField, HideLabel]
		[Title("ResourcesKey")]
		public ResourcesKey playMapKey;

		[Title("MapStageInfo")]
		[SerializeField, HideLabel]
		public IMapSetting.MapStageInfo playMapStageInfo;

		public ResourcesKey PlayMapKey => playMapKey;
		public IMapSetting.MapStageInfo PlayMapStageInfo => playMapStageInfo;
	}
}
