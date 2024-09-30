using System;
using System.Collections.Generic;
using System.Linq;

using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;
using UnityEngine.AI;

namespace BC.Map
{

	public class MapPathPoint : ComponentBehaviour, IMapPathPoint
	{
		private MapAnchor mapAnchor;
		[SerializeField]
		private bool isBrakePath;
		[SerializeField]
		private float defaultPathCost = 1f;
		public Vector3 closeNavMeshPosition;
		public MapPathPoint[] nextPathPointList;

		public IMapAnchor ThisAnchor => mapAnchor;
		public float DistancePerCost => IsBrakePath ? float.PositiveInfinity : defaultPathCost;
		public bool IsBrakePath { get => isBrakePath; private set => isBrakePath=value; }

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
			//float Distance = diraction.Distance;
			//diraction = diraction.normalized;
			//Ray ray = new Ray(a, diraction);
			//
			////List<MapPathPoint> asyncPathPointList = nextPathPointList.ToList();
			//if(Physics.Raycast(ray, out var hit, Distance, TagAndLayer.GetHitLayerMask(TagAndLayer.MapAnchor), QueryTriggerInteraction.Collide))
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
		public bool CalculatePath(IMapPathPoint target, out IMapPathNode _pathNode)
		{
			bool result = _CalculatePath((MapPathPoint)target, out var pathNode);
			_pathNode = pathNode;
			if(result)
			{
				var start = pathNode;
				var ended = pathNode.Last();
				foreach(var item in pathNode)
				{
					item.SetStartAndLast(start, ended);
				}
			}

			return result;
		}
		private bool _CalculatePath(MapPathPoint target, out MapPathNode pathNode)
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
				var endedNode = new MapPathNode(endedPoint);
				MapPathNode.LinkNode(pathNode, endedNode);
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
				var point = node.ThisPoint as MapPathPoint;
				serching.RemoveAt(0);
				serched.Add(node.ThisPoint as MapPathPoint);
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

					// �̵��� ��ȿ�� ������� �˻�
					if(nextPoint.IsBrakePath) continue;

					// �̹� Ž���� ������� Ȯ��
					bool isSearched = false;
					foreach(MapPathPoint searchedPoint in serched)
					{
						if(searchedPoint == nextPoint)
						{
							isSearched = true;
							break;
						}
					}

					// �̹� Ž���� ��� �̸� ����
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
			MapPathNode first = finded;
			while(!first.IsStart)
			{
				MapPathNode.LinkNode(first.PrevNode as MapPathNode, first);
				first = first.PrevNode as MapPathNode;
			}
			pathNode = first;
			return true;
		}
	}
}
