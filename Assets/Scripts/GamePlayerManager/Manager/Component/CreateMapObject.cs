using BC.Base;
using BC.LowLevelAI;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = BC.Base.Debug;

namespace BC.GamePlayerManager
{
	public class CreateMapObject : ComponentBehaviour, IStartSetup
	{
		public bool IsCompleteSetting { get; private set; }
		public StartMapSetting MapSetting { get; internal set; }


		[SerializeField, HideLabel]
		[FoldoutGroup("MapNavmesModel")]
		private ResourcesKey mapNavmeshKey;
		[SerializeField, HideLabel]
		[FoldoutGroup("MapAnchorModel")]
		private ResourcesKey mapAnchorKey;

		private NavMeshConnectComputer mapNavmesh;
		private MapPathPointComputer mapAnchor;

		public override void BaseEnable()
		{
			IsCompleteSetting = false;


			mapNavmesh = ThisContainer.GetComponent<NavMeshConnectComputer>();
			mapAnchor = ThisContainer.GetComponent<MapPathPointComputer>();

			if(mapNavmesh == null)
			{
				if(MapSetting != null)
				{
					mapNavmeshKey = MapSetting.MapNavmeshKey;
				}
				mapNavmeshKey.AsyncInstantiate<GameObject>(this, thisObject =>
				{
					if(thisObject == null)
					{
						Debug.LogError("NavMeshConnectComputer Instantiate Is Null");
						return;
					}
					thisObject.transform.ResetLcoalPose(ThisTransform);
					mapNavmesh = thisObject.GetComponent<NavMeshConnectComputer>();
					thisObject.SetActive(true);
				});
			}
			if(mapAnchor == null)
			{
				if(MapSetting != null)
				{
					mapAnchorKey = MapSetting.MapAnchorKey;
				}
				mapAnchorKey.AsyncInstantiate<GameObject>(this, thisObject =>
				{
					if(thisObject == null)
					{
						Debug.LogError("MapPathPointComputer Instantiate Is Null");
						return;
					}
					thisObject.transform.ResetLcoalPose(ThisTransform);
					mapAnchor = thisObject.GetComponent<MapPathPointComputer>();
					thisObject.SetActive(true);
				});
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
