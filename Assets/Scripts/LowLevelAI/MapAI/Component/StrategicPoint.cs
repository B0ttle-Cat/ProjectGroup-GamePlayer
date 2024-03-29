using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

namespace BC.LowLevelAI
{
	[Serializable]
	public class StrategicPath
	{
		public StrategicPoint from;
		public StrategicPoint to;
		public MapPathPoint[] path;

		public float PointPerTime => to.pointPerTime;
		public float WayCost => path.Max(item => item.WayCost);
		public float StayPoint => 0f;
		public float MovePoint => 1f;
	}
	public class StrategicPoint : ComponentBehaviour
	{
		public int pointPerTime;
		public MapPathPoint thiaPathpoint;
		public List<StrategicPath> nextStrategicPoint;
		public Action onUpdateNextStrategicPoint;

		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		public override void BaseAwake()
		{
			base.BaseAwake();

			thiaPathpoint = ThisContainer.GetComponent<MapPathPoint>();
		}


		internal void StrategicPointUpdate()
		{
			Dictionary<StrategicPoint, MapPathPoint[]> strategicPoints = new Dictionary<StrategicPoint, MapPathPoint[]>();
			List<MapPathPoint> root = new List<MapPathPoint>();
			SerachPathpoint(null, thiaPathpoint, root, strategicPoints);

			nextStrategicPoint = new List<StrategicPath>();
			foreach(var item in strategicPoints)
			{
				nextStrategicPoint.Add(new StrategicPath()
				{
					from = this,
					to = item.Key,
					path = item.Value,
				});
			}

			void SerachPathpoint(MapPathPoint prev, MapPathPoint current, List<MapPathPoint> root, Dictionary<StrategicPoint, MapPathPoint[]> result)
			{
				root.Add(current);
				var list = current.nextPathpointList;
				int Length = list.Length;
				for(int i = 0 ; i < Length ; i++)
				{
					var point = list[i];

					if(prev == point || point.breakPass) continue;
					if(point.ThisContainer.TryGetComponent<StrategicPoint>(out var strategicPoint))
					{
						root.Add(point);
						result.Add(strategicPoint, root.ToArray());
						root.Remove(point);
					}
					else
					{
						SerachPathpoint(current, point, root, result);
					}

				}
				root.Remove(current);
			}
		}


		public bool FindTargetPath(StrategicPoint target, out List<StrategicPath> resultPath, out float totalCost)
		{
			resultPath = new List<StrategicPath>();
			totalCost = 0f;

			var start = this;
			if(start == null || target == null || resultPath == null) return false;
			if(start == target)
			{
				return true;
			}

			HashSet<StrategicPoint> history = new HashSet<StrategicPoint>();
			List<(List<StrategicPath> path, float cost)> serchTree = new List<(List<StrategicPath> path, float cost)>();

			history.Add(start);
			int count = start.nextStrategicPoint.Count;
			for(int i = 0 ; i < count ; i++)
			{
				var path = start.nextStrategicPoint[i];

				if(history.Contains(path.to)) continue;

				serchTree.Add((new List<StrategicPath>() { path }, path.WayCost));
			}

			(List<StrategicPath> path, float cost) result = default;
			bool isEnded = false;
			while(true)
			{
				_SerchStrategicPoint();
				if(isEnded) break;
			}

			if(isEnded)
			{
				var resultValue = result;
				resultPath.AddRange(resultValue.path);
				totalCost = resultValue.cost;

				return true;
			}
			return false;


			void _SerchStrategicPoint()
			{
				int Count = serchTree.Count;
				if(Count == 0)
				{
					isEnded = true;
					return;
				}

				serchTree.Sort((a, b) => a.cost.CompareTo(b.cost));

				var minCostPath = serchTree[0].path[^1];
				if(minCostPath.to == target)
				{
					result = serchTree[0];
					isEnded = true;
					return;
				}
				else
				{
					var next = minCostPath.to;
					history.Add(next);
					int count = next.nextStrategicPoint.Count;
					for(int i = 0 ; i < count ; i++)
					{
						var path = next.nextStrategicPoint[i];

						if(history.Contains(path.to)) continue;

						serchTree.Add((new List<StrategicPath>() { path }, minCostPath.WayCost + path.WayCost));
					}
					serchTree.RemoveAt(0);
				}
			}

		}
	}
}
