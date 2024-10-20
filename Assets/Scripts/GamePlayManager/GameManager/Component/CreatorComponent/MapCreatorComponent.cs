using BC.Base;
using BC.Map;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = BC.Base.Debug;

namespace BC.GamePlayManager
{
	public class MapCreatorComponent : ComponentBehaviour, IStartSetup, IMapCreatorComponent
	{
		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				if(MapSetting == null || MapNavmesh == null || MapAnchor == null) return false;
				IsCompleteSetting = true;
				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}
		public IMapSetting MapSetting { get; set; }

		[SerializeField, HideLabel]
		[FoldoutGroup("PlayMap")]
		private ResourcesKey playMapKey;

		[SerializeField, ReadOnly]
		private NavMeshConnectComputer mapNavmesh = null;
		[SerializeField, ReadOnly]
		private MapPathPointComputer mapAnchor = null;

		public NavMeshConnectComputer MapNavmesh { get => mapNavmesh; private set => mapNavmesh=value; }
		public MapPathPointComputer MapAnchor { get => mapAnchor; private set => mapAnchor=value; }

		public override async void BaseAwake()
		{
			IsCompleteSetting = false;
			while(MapSetting == null)
			{
				await Awaitable.NextFrameAsync();
			}

			MapNavmesh = ThisContainer.GetComponentInChild<NavMeshConnectComputer>();
			MapAnchor = ThisContainer.GetComponentInChild<MapPathPointComputer>();
			if(MapNavmesh == null || MapAnchor == null)
			{
				if(MapNavmesh != null) Destroy(MapNavmesh);
				if(MapAnchor != null) Destroy(MapAnchor);

				playMapKey = MapSetting.PlayMapKey;
				var thisObject  = await playMapKey.AsyncInstantiate<GameObject>(this);

				if(thisObject == null)
				{
					Debug.LogError("NavMeshConnectComputer Instantiate Is Null");
					return;
				}
				thisObject.transform.ResetLcoalPose(ThisTransform);
				MapNavmesh = thisObject.GetComponentInChildren<NavMeshConnectComputer>();
				MapAnchor = thisObject.GetComponentInChildren<MapPathPointComputer>();
				thisObject.SetActive(true);
			}
			else
			{
				playMapKey = MapSetting.PlayMapKey;
			}
		}
	}
}
