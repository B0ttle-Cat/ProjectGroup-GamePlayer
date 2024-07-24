using BC.Base;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = BC.Base.Debug;

namespace BC.GamePlayerManager
{
	public class CreateMapObject : ComponentBehaviour, IStartSetup
	{
		public bool IsCompleteSetting { get; set; }
		public StartMapSetting MapSetting { get; internal set; }


		[SerializeField, HideLabel]
		[FoldoutGroup("PlayMap")]
		private ResourcesKey playMapKey;

		[SerializeField]
		private NavMeshConnectComputer mapNavmesh;
		[SerializeField]
		private MapPathPointComputer mapAnchor;

		public override void BaseEnable()
		{
			IsCompleteSetting = false;

			mapNavmesh = ThisContainer.GetComponentInChild<NavMeshConnectComputer>();
			mapAnchor = ThisContainer.GetComponentInChild<MapPathPointComputer>();

			if(mapNavmesh == null || mapAnchor == null)
			{
				if(MapSetting != null)
				{
					playMapKey = MapSetting.playMapKey;
				}
				playMapKey.AsyncInstantiate<GameObject>(this, thisObject => {
					if(thisObject == null)
					{
						Debug.LogError("NavMeshConnectComputer Instantiate Is Null");
						return;
					}
					thisObject.transform.ResetLcoalPose(ThisTransform);
					mapNavmesh = thisObject.GetComponentInChildren<NavMeshConnectComputer>();
					mapAnchor = thisObject.GetComponentInChildren<MapPathPointComputer>();
					thisObject.SetActive(true);
				});
			}
			else
			{
				if(MapSetting != null)
				{
					playMapKey = MapSetting.playMapKey;
				}
			}
		}

		public override void BaseUpdate()
		{
			OnUpdateSetting();
		}

		public void OnStartSetting()
		{
			enabled = true;
		}

		public void OnStopSetting()
		{
			enabled = false;
		}

		public void OnUpdateSetting()
		{
			if(IsCompleteSetting)
			{
				OnStopSetting();
				return;
			}

			if(mapNavmesh == null || mapAnchor == null)
			{
				return;
			}

			IsCompleteSetting = true;
		}
	}
}
