using BC.Base;
using BC.Map;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = BC.Base.Debug;

namespace BC.GamePlayerManager
{
	public class CreateMapObject : ComponentBehaviour, IStartSetup
	{
		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				if(MapSetting == null || mapNavmesh == null || mapAnchor == null) return false;
				IsCompleteSetting = true;
				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}
		public StartMapSetting MapSetting { get; internal set; }


		[SerializeField, HideLabel]
		[FoldoutGroup("PlayMap")]
		private ResourcesKey playMapKey;

		[SerializeField, ReadOnly]
		private NavMeshConnectComputer mapNavmesh = null;
		[SerializeField, ReadOnly]
		private MapPathPointComputer mapAnchor = null;

		public override async void BaseAwake()
		{
			IsCompleteSetting = false;
			while(MapSetting == null)
			{
				await Awaitable.NextFrameAsync();
			}

			mapNavmesh = ThisContainer.GetComponentInChild<NavMeshConnectComputer>();
			mapAnchor = ThisContainer.GetComponentInChild<MapPathPointComputer>();
			if(mapNavmesh == null || mapAnchor == null)
			{
				if(mapNavmesh != null) Destroy(mapNavmesh);
				if(mapAnchor != null) Destroy(mapAnchor);

				playMapKey = MapSetting.playMapKey;
				var thisObject  = await playMapKey.AsyncInstantiate<GameObject>(this);

				if(thisObject == null)
				{
					Debug.LogError("NavMeshConnectComputer Instantiate Is Null");
					return;
				}
				thisObject.transform.ResetLcoalPose(ThisTransform);
				mapNavmesh = thisObject.GetComponentInChildren<NavMeshConnectComputer>();
				mapAnchor = thisObject.GetComponentInChildren<MapPathPointComputer>();
				thisObject.SetActive(true);
			}
			else
			{
				playMapKey = MapSetting.playMapKey;
			}
		}
	}
}
