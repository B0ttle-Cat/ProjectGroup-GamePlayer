using System.Collections.Generic;

using BC.ODCC;

using Unity.AI.Navigation;

using UnityEngine;

using static BC.LowLevelAI.NavMeshConnectComputer;

namespace BC.LowLevelAI
{
	public class MapAICellData : DataObject
	{
		public NavMeshSurface navMeshSurface;
		public float navMeshTileSize => navMeshSurface.tileSize * navMeshSurface.voxelSize;


		public Dictionary<Vector3Int,List<Triangle>> trianglesTile = new Dictionary<Vector3Int,List<Triangle>>();
		public Dictionary<Triangle, LinkTriangle[]> trianglesToLink = new Dictionary<Triangle, LinkTriangle[]>();
		//public Dictionary<Triangle, MapWaypoint> closeWaypoint = new Dictionary<Triangle, MapWaypoint>();


		public bool IndexToTriangles(Vector3Int index, out List<Triangle> triangles)
		{
			return trianglesTile.TryGetValue(index, out triangles);
		}
		public Vector3Int GetCellIndex(Vector3 position)
		{
			float _navMeshTileSize = navMeshTileSize;
			int xIndex = Mathf.FloorToInt(position.x / _navMeshTileSize);
			int yIndex = Mathf.FloorToInt(position.y / _navMeshTileSize);
			int zIndex = Mathf.FloorToInt(position.z / _navMeshTileSize);

			return new Vector3Int(xIndex, yIndex, zIndex);
		}
	}
}
