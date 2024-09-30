using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using Unity.AI.Navigation;

using UnityEngine;

namespace BC.Map
{
	public class MapCellData : DataObject
	{
		public NavMeshSurface navMeshSurface;
		public float navMeshTileSize => navMeshSurface.tileSize * navMeshSurface.voxelSize;
		public float navMeshSerchRange => 10f;


		[ShowInInspector,ReadOnly]
		public Dictionary<Vector3Int,List<Triangle>> trianglesTile = new Dictionary<Vector3Int,List<Triangle>>();
		[ShowInInspector,ReadOnly]
		public Dictionary<Triangle, LinkRayTriangle[]> trianglesToLink = new Dictionary<Triangle, LinkRayTriangle[]>();
		[ShowInInspector,ReadOnly]
		public Dictionary<Triangle, IMapPathPoint> trianglesClosedPoint = new Dictionary<Triangle, IMapPathPoint>();


		public bool IndexToTriangles(Vector3Int index, out List<Triangle> triangles)
		{
			return trianglesTile.TryGetValue(index, out triangles);
		}
		public Vector3Int VectorToIndex(Vector3 position)
		{
			float _navMeshTileSize = navMeshTileSize;
			int xIndex = Mathf.FloorToInt(position.x / _navMeshTileSize);
			int yIndex = Mathf.FloorToInt(position.y / _navMeshTileSize);
			int zIndex = Mathf.FloorToInt(position.z / _navMeshTileSize);

			return new Vector3Int(xIndex, yIndex, zIndex);
		}
	}
}
