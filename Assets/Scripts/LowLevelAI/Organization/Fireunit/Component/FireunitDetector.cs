using System.Collections.Generic;

using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitDetector : ComponentBehaviour
	{
		public CapsuleCollider thisCollider;

		public Vector3 DetectorPosition { get; private set; }
		public Vector3 DetectorPositionXZ { get; private set; }
		public Vector3 LookForward { get; private set; }
		public Vector3 LookForwardXZ { get; private set; }
		public float AidDetectorRange { get; private set; }
		public float AntiDetectorRange { get; private set; }

		public Vector3Int NavTitleIndex { get; private set; }


		public float ColliderRadius { get; private set; }

		public DetectorRangeData DetectorRangeData { get; private set; }


		public Triangle ThisTriangle { get; private set; } = default;
		public Dictionary<Vector3Int, List<LinkRayTriangle>> ThisLinkTriangles { get; private set; } = new Dictionary<Vector3Int, List<LinkRayTriangle>>();


		public override void BaseAwake()
		{
			base.BaseAwake();
		}



		public override void BaseDestroy()
		{
			base.BaseDestroy();
		}
		public override void BaseEnable()
		{
			base.BaseEnable();

			thisCollider = ThisObject.GetComponent<CapsuleCollider>();
			if(thisCollider == null)
			{
				thisCollider = ThisObject.gameObject.AddComponent<CapsuleCollider>();
			}
			thisCollider.isTrigger = false;
			thisCollider.providesContacts = false;
			thisCollider.material = null;
			thisCollider.radius = 0.5f;
			thisCollider.height = 2f;
			thisCollider.direction = 1;
			thisCollider.center = Vector3.up;

			thisCollider.layerOverridePriority = 0;
			thisCollider.includeLayers = 0;
			thisCollider.excludeLayers = 0;

			ColliderRadius = thisCollider.radius;

			OnUpdateDatas(ThisContainer.DataList);
		}
		public override void BaseDisable()
		{
			base.BaseDisable();

			DetectorRangeData = null;
		}
		public override void BaseStart()
		{

		}

		protected override void OnUpdateDatas(DataObject[] update)
		{
			AidDetectorRange = 0f;
			AntiDetectorRange = 0f;
			int Length = update.Length;
			for(int i = 0 ; i < Length ; i++)
			{
				var data = update[i];
				if(data is null)
				{
					continue;
				}
				else if(data is DetectorRangeData rangeData)
				{
					DetectorRangeData = rangeData;
				}
				else if(data is AidDetectorRangeData aidData)
				{
					AidDetectorRange += aidData.detectRadius;
				}
				else if(data is AntiDetectorRangeData antiData)
				{
					AntiDetectorRange += antiData.detectRadius;
				}
			}
		}

		internal void UpdateDetector(NavMeshConnectComputer navMeshConnectComputer, MapCellData mapAICellData)
		{
			Vector3 position = ThisTransform.position;
			DetectorPosition = position;
			position.y = 0f;
			DetectorPositionXZ = position;

			Vector3 forward = ThisTransform.forward;
			LookForward = forward;
			forward.y = 0;
			LookForwardXZ = forward.normalized;

			if(navMeshConnectComputer != null)
			{
				NavTitleIndex = mapAICellData.VectorToIndex(DetectorPosition);
				if(ThisLinkTriangles == null)
				{
					ThisLinkTriangles = new Dictionary<Vector3Int, List<LinkRayTriangle>>();
				}
				if(navMeshConnectComputer != null && !ThisTriangle.IsPointInTriangle(DetectorPosition) && navMeshConnectComputer.FindPointInTriangle(DetectorPosition, out var thisTriangle, out var linkList))
				{
					if(ThisTriangle.Index != thisTriangle.Index)
					{
						ThisTriangle = thisTriangle;

						ThisLinkTriangles.Clear();
						int length = linkList.Length;
						for(int i = 0 ; i < length ; i++)
						{
							LinkRayTriangle link = linkList[i];
							if(link.GetOther(thisTriangle, out var otherTriangle))
							{
								var otherNavTitleIndex = mapAICellData.VectorToIndex(otherTriangle.Center);

								if(!ThisLinkTriangles.TryGetValue(otherNavTitleIndex, out var linlValue))
								{
									linlValue = new List<LinkRayTriangle>();
									ThisLinkTriangles.Add(otherNavTitleIndex, linlValue);
								}
								linlValue.Add(link);
							}
						}
					}
				}
			}
		}
	}
}
