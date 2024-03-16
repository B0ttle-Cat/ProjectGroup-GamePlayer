using System.Collections.Generic;

using UnityEngine;

using static BC.LowLevelAI.NavMeshConnectComputer;


namespace BC.LowLevelAI
{
	public partial class FactionDetector//RaycastComputing
	{
		private  List<int> isPerfectIndex;
		public void RaycastStart()
		{
			if(isPerfectIndex == null)
				isPerfectIndex = new List<int>();
			isPerfectIndex.Clear();
		}
		public void RaycastEnded()
		{
			isPerfectIndex.Clear();
		}
		public bool RaycastComputing(FireunitDetector myDetector, FireunitDetector targetDetectors)
		{
			//int NavMeshRaycastMask = TagAndLayer.GetHitLayerMask(TagAndLayer.NavMeshRaycast);

			Vector3 thisPos = myDetector.DetectorPosition;

			//Triangle ThisTriangle = myDetector.ThisTriangle;
			Dictionary<Vector3Int, List<LinkTriangle>> ThisLinkTriangles = myDetector.ThisLinkTriangles;

			if(ThisLinkTriangles == null) return false;
			if(ThisLinkTriangles.Count == 0) return false;


			Vector3 targetPosition = targetDetectors.DetectorPosition;
			Vector3Int targetTileIndex = targetDetectors.NavTitleIndex;
			Triangle TargetTriangle = targetDetectors.ThisTriangle;
			int TargetTriangleIndex = TargetTriangle.Index;
			if(isPerfectIndex.Contains(TargetTriangleIndex)) return true;

			if(ThisLinkTriangles.TryGetValue(targetTileIndex, out var linkTriangles))
			{
				int Count = linkTriangles.Count;
				for(int i = 0 ; i < Count ; i++)
				{
					var link = linkTriangles[i];
					if(link.IsThis(TargetTriangleIndex))
					{
						if(link.IsPerfectLink)
						{
							isPerfectIndex.Add(TargetTriangleIndex);
							return true;
						}
						else
						{
							return !Raycast(thisPos, targetPosition);
						}
					}
				}
			}
			return false;

			bool Raycast(Vector3 thisPos, Vector3 targetPos)
			{
				thisPos += Vector3.up;
				targetPos += Vector3.up;
				Vector3 distance = targetPos  - thisPos;
				Ray ray = new Ray(thisPos, distance.normalized);
				float length = distance.magnitude;

				if(Physics.Raycast(thisPos + Vector3.up, targetPos + Vector3.up, out var hit, length, -1))
				{
					return true;
				}
				return false;
			}
		}
	}
}
