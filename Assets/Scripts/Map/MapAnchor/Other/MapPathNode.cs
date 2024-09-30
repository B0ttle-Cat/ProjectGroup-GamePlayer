using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.Map
{
	[Serializable]
	public class MapPathNode : IEnumerable<MapPathNode>, IMapPathNode
	{
		[SerializeField, ReadOnly]
		private MapPathPoint mapPathPoint;
		[SerializeField, ReadOnly]
		private MapPathNode prevNode;
		[SerializeField, ReadOnly]
		private MapPathNode nextNode;

		private float pathCost; // prevNode 를 거쳐 이곳까지 오는 누적 비용
		private float nextCost; // 이곳에서 next 로 가는 비용

		public MapPathNode()
		{
			mapPathPoint = null;
			prevNode = null;
			nextNode = null;
		}
		public MapPathNode(MapPathPoint mapPathPoint, MapPathNode prevNode = null)
		{
			this.mapPathPoint=mapPathPoint;
			this.prevNode = prevNode;
			if(prevNode != null)
			{
				float distance = Vector3.Distance(prevNode.ThisPoint.ThisPosition(), this.ThisPoint.ThisPosition());
				float distancePerCost = (prevNode.DistancePerCost + this.DistancePerCost) * 0.5f;

				this.pathCost = prevNode.pathCost + (distance * distancePerCost);
			}
			else
			{
				this.pathCost = 0f;
			}
			nextNode = null;
		}
		public MapPathNode(MapPathNode copy)
		{
			this.mapPathPoint = copy.mapPathPoint;
			this.prevNode = copy.prevNode;
			this.nextNode = copy.nextNode;
			this.pathCost = copy.pathCost;
			this.nextCost = copy.nextCost;
		}
		public static void LinkNode(MapPathNode prev, MapPathNode next)
		{
			if(prev != null)
			{
				prev.nextNode = next;
				if(next != null)
				{
					float distance = Vector3.Distance(prev.ThisPoint.ThisPosition(), next.ThisPoint.ThisPosition());
					float distancePerCost = (prev.DistancePerCost + next.DistancePerCost) * 0.5f;

					prev.nextCost = (distance * distancePerCost);
				}
				else
				{
					prev.nextCost = 0f;
				}
			}
			if(next!= null)
			{
				next.prevNode = prev;
				if(prev != null)
				{
					float distance = Vector3.Distance(prev.ThisPoint.ThisPosition(), next.ThisPoint.ThisPosition());
					float distancePerCost = (prev.DistancePerCost + next.DistancePerCost) * 0.5f;

					next.pathCost = prev.pathCost + (distance * distancePerCost);
				}
				else
				{
					next.pathCost = 0f;
				}
			}
		}

		public IMapPathPoint ThisPoint => mapPathPoint;
		public IMapPathNode PrevNode => prevNode;
		public IMapPathNode NextNode => nextNode;

		public IMapPathNode StartNode { get; private set; }
		public IMapPathNode EndedNode { get; private set; }

		public void SetStartAndLast(MapPathNode startNode, MapPathNode endedNode)
		{
			StartNode = startNode;
			EndedNode = endedNode;
		}

		public bool IsStart => PrevNode == null;
		public bool IsEnded => NextNode == null;
		public bool IsPath => !IsStart && !IsEnded;

		public float DistancePerCost => ThisPoint == null ? 0 : ThisPoint.DistancePerCost;

		public float PathCost => pathCost;
		public float NextCost => nextCost;

		public IEnumerator<MapPathNode> GetEnumerator()
		{
			MapPathNode currentNode = this;
			while(currentNode != null)
			{
				yield return currentNode;
				currentNode = (MapPathNode)currentNode.NextNode;
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

#if UNITY_EDITOR
		public void OnDrawGizmos(Vector3 upOffset)
		{
			var list = this.ToList();
			int count = list.Count;
			for(int i = 0 ; i < count - 1 ; i++)
			{
				var nodeA = list[i];
				var nodeB = list[i+1];

				Gizmos.DrawLine(nodeA.ThisPoint.ThisPosition() + upOffset, nodeB.ThisPoint.ThisPosition() + upOffset);
			}
		}
#endif
	}

}
