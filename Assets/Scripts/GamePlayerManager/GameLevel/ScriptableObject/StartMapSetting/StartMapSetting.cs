using BC.Base;
using BC.GameBaseInterface;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "StartMapSetting", menuName = "BC/new StartMapSetting")]
	public class StartMapSetting : ScriptableObject, IMapModelData
	{
		[SerializeField, HideLabel]
		[HorizontalGroup("Model"), FoldoutGroup("Model/MapNavmesModel")]
		private ResourcesKey mapNavmeshKey;
		[SerializeField, HideLabel]
		[HorizontalGroup("Model"), FoldoutGroup("Model/MapAnchorModel")]
		private ResourcesKey mapAnchorKey;

		public ResourcesKey MapNavmeshKey { get => mapNavmeshKey; set => mapNavmeshKey = value; }
		public ResourcesKey MapAnchorKey { get => mapAnchorKey; set => mapAnchorKey = value; }
	}
}
