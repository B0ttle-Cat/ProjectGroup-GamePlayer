using UnityEngine;

namespace BC.OdccBase
{
	public interface IMapPathNode
	{
		public IMapPathPoint ThisPoint { get; }
		public IMapPathNode PrevNode { get; }
		public IMapPathNode NextNode { get; }

		public IMapPathNode StartNode { get; }
		public IMapPathNode EndedNode { get; }

		public bool IsStart { get; }
		public bool IsEnded { get; }
		public bool IsPath { get; }

		public float DistancePerCost { get; }

		public float PathCost { get; }
		public float NextCost { get; }

#if UNITY_EDITOR
		public void OnDrawGizmos(Vector3 upOffset);
#endif
	}
}
