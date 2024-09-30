using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IMapAnchor : IOdccObject
	{
		public Vector3 ThisPosition();
		public bool InSidePosition(Vector3 position);
		public Vector3 ClosestPoint(Vector3 position, out float distance);
	}
}
