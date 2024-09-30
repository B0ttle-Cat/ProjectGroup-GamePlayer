using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IMapPathPoint : IOdccComponent
	{
		public IMapAnchor ThisAnchor { get; }
		public float DistancePerCost { get; }
		public bool IsBrakePath { get; }
		public Vector3 ThisPosition();
		public Vector3 CloseNavMeshPosition();
		public bool InSidePosition(Vector3 position);
		public Vector3 ClosestPoint(Vector3 position, out float distance);
		public bool CalculatePath(IMapPathPoint target, out IMapPathNode _pathNode);
	}
}
