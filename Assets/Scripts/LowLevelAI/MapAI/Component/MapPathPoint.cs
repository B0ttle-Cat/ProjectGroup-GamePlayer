using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

namespace BC.LowLevelAI
{
	[Serializable]
	public class MapPathNode : IEnumerable<MapPathNode>
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

		public MapPathPoint ThisPoint => mapPathPoint;
		public MapPathNode PrevNode => prevNode;
		public MapPathNode NextNode => nextNode;

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
				currentNode = currentNode.NextNode;
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

#if UNITY_EDITOR
		internal void OnDrawGizmos(Vector3 upOffset)
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

	public class MapPathPoint : ComponentBehaviour
	{
		private MapAnchor mapAnchor;
		[SerializeField]
		private bool isBrakePath;
		[SerializeField]
		private float defaultPathCost = 1f;
		public Vector3 closeNavMeshPosition;
		public MapPathPoint[] nextPathPointList;

		public float DistancePerCost => IsBrakePath ? float.PositiveInfinity : defaultPathCost;
		public bool IsBrakePath { get => isBrakePath; set => isBrakePath=value; }

		public override void BaseValidate()
		{
			base.BaseValidate();
			mapAnchor = ThisObject as MapAnchor;
		}

		public override void BaseAwake()
		{
			base.BaseAwake();
			mapAnchor = ThisObject as MapAnchor;
			SetNavMeshPosition();
		}

		public override void BaseEnable()
		{
			nextPathPointList = new MapPathPoint[0];

			base.BaseEnable();
		}

		internal void SetNavMeshPosition()
		{
			Vector3 thisPosition = ThisTransform.position;
			if(NavMesh.SamplePosition(thisPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
			{
				closeNavMeshPosition = hit.position;
			}
			else
			{
				closeNavMeshPosition = thisPosition;
			}
			ThisTransform.position = closeNavMeshPosition;
		}

		internal void ConnectingNeighbor(MapPathPoint targetPathPoint, List<MapPathPoint> asyncPathPointList)
		{
			if(targetPathPoint == null) return;

			Vector3 start = closeNavMeshPosition;
			Vector3 ended = targetPathPoint.closeNavMeshPosition;

			int startAnchorIndex = ThisContainer.GetData<MapAnchorData>().anchorIndex;
			int endedAnchorIndex = targetPathPoint.ThisContainer.GetData<MapAnchorData>().anchorIndex;

			NavMeshPath path = new NavMeshPath();
			bool getPath = NavMesh.CalculatePath(start, ended, NavMesh.AllAreas, path);
			if(!getPath)
			{
				return;
			}

			if(path.status != NavMeshPathStatus.PathComplete) return;

			var corners = path.corners;

			int length = corners.Length;
			bool passThroughMapAnchor = false;
			for(int i = 0 ; i < length - 1 ; i++)
			{
				var pointA = corners[i];
				var pointB = corners[i+1];

				Vector3 diraction = pointB - pointA;
				float distance = diraction.magnitude;
				diraction = diraction.normalized;
				Ray ray = new Ray(pointA, diraction);
				var hits = Physics.RaycastAll(ray, distance, TagAndLayer.GetIndexToMask(TagAndLayer.MapAnchor), QueryTriggerInteraction.Collide);

				passThroughMapAnchor = false;
				if(hits.Length > 0)
				{
					int hitLength = hits.Length;
					for(int ii = 0 ; ii < hitLength ; ii++)
					{
						var hit = hits[ii];
						if(!hit.collider.isTrigger) continue;
						if(hit.collider.TryGetComponent<MapAnchor>(out var hitAnchor))
						{
							if(hitAnchor.ThisContainer.TryGetData<MapAnchorData>(out var data)
								&& (data.anchorIndex == endedAnchorIndex || data.anchorIndex == startAnchorIndex))
							{
								passThroughMapAnchor = false;
							}
							else
							{
								passThroughMapAnchor = true;
								break;
							}
						}
					}
				}

				if(passThroughMapAnchor)
				{
					break;
				}
			}

			if(!passThroughMapAnchor)
			{
				asyncPathPointList.Add(targetPathPoint);
			}

			//Vector3 diraction = b - a;
			//float distance = diraction.distance;
			//diraction = diraction.normalized;
			//Ray ray = new Ray(a, diraction);
			//
			////List<MapPathPoint> asyncPathPointList = nextPathPointList.ToList();
			//if(Physics.Raycast(ray, out var hit, distance, TagAndLayer.GetHitLayerMask(TagAndLayer.MapAnchor), QueryTriggerInteraction.Collide))
			//{
			//	MapPathPoint mapPathPoint = hit.collider.gameObject.GetComponentInParent<MapPathPoint>();
			//
			//	if(mapPathPoint is not null)
			//	{
			//		if(mapPathPoint == targetPathPoint)
			//		{
			//			asyncPathPointList.Add(targetPathPoint);
			//		}
			//	}
			//}
		}

		internal void ConnectNeighborEnded(List<MapPathPoint> asyncPathPointList)
		{
			nextPathPointList = asyncPathPointList.ToArray();
		}

		public Vector3 ThisPosition()
		{
			return mapAnchor.ThisPosition();
		}
		public Vector3 CloseNavMeshPosition()
		{
			return closeNavMeshPosition;
		}
		public bool InSidePosition(Vector3 position)
		{
			return mapAnchor.InSidePosition(position);
		}
		public Vector3 ClosestPoint(Vector3 position, out float distance)
		{
			return mapAnchor.ClosestPoint(position, out distance);
		}

		public bool CalculatePath(MapPathPoint target, out MapPathNode pathNode)
		{
			pathNode = null;
			if(target == null || target.IsBrakePath) return false;
			if(this == target)
			{
				pathNode = new MapPathNode(this);
				return true;
			}
			var startPoint = this;
			var endedPoint = target;

			if(startPoint.nextPathPointList.Contains(endedPoint))
			{
				pathNode = new MapPathNode(this);
				MapPathNode.LinkNode(pathNode, new MapPathNode(endedPoint));
				return true;
			}

			List<MapPathNode> serching = new List<MapPathNode>();
			List<MapPathPoint> serched = new List<MapPathPoint>();
			serching.Add(new MapPathNode(this));
			MapPathNode finded = null;
			while(finded == null || serching.Count == 0)
			{
				_CalculatePath();
			}
			void _CalculatePath()
			{
				if(serching.Count == 0) return;
				var node = serching[0];
				var point = node.ThisPoint;
				serching.RemoveAt(0);
				serched.Add(node.ThisPoint);
				if(point == target)
				{
					finded = node;
					return;
				}

				MapPathPoint[] nextPointList = point.nextPathPointList;
				int length = nextPointList.Length;
				for(int i = 0 ; i < length ; i++)
				{
					MapPathPoint nextPoint = nextPointList[i];

					// 이동이 유효한 노드인지 검사
					if(nextPoint.IsBrakePath) continue;

					// 이미 탐색한 노드인지 확인
					bool isSearched = false;
					foreach(MapPathPoint searchedPoint in serched)
					{
						if(searchedPoint == nextPoint)
						{
							isSearched = true;
							break;
						}
					}

					// 이미 탐색한 노드 이면 무시
					if(isSearched) continue;

					MapPathNode nextNode = new MapPathNode(nextPoint, node);

					float cost = nextNode.PathCost;
					int indexToInsert = 0;
					int serchingCount = serching.Count;
					for(int j = 0 ; j < serchingCount ; j++)
					{
						if(cost < serching[j].PathCost)
						{
							indexToInsert = j;
							break;
						}
						indexToInsert++;
					}
					serching.Insert(indexToInsert, nextNode);
				}
			}
			if(finded == null) return false;

			Stack<MapPathNode> stack = new Stack<MapPathNode>();
			while(!finded.IsStart)
			{
				stack.Push(finded);
				finded = finded.PrevNode;
			}
			pathNode = finded;
			while(stack.Count > 0)
			{
				var next = stack.Pop();
				MapPathNode.LinkNode(finded, next);
				finded = next;
			}
			return true;
		}

		internal Vector3[] GetRandomAroundPosition(int divisionCount = 0)
		{
			if(divisionCount == 0)
			{
				return new Vector3[1] { ThisPosition() };
			}

			Vector3[] points = new Vector3[divisionCount];

			float pointsPerDivision = 360f / divisionCount;

			for(int i = 0 ; i < divisionCount ; i++)
			{
				// 각 구역을 등분하여 시작 각도와 끝 각도 계산
				float startAngle = i * pointsPerDivision;
				float endAngle = (i + 1) * pointsPerDivision;

				// 각 구역에서 랜덤한 각도 선택
				float angle = Random.Range(startAngle, endAngle);

				// 선택된 각도에 해당하는 위치 계산
				Quaternion rotation = Quaternion.Euler(0, angle, 0);
				Vector3 direction = rotation * Vector3.forward;
				Vector3 point = ThisPosition() + direction * Random.Range(0.5f, 2.5f);

				points[i] = (point);
			}

			return points;
		}
	}
}
