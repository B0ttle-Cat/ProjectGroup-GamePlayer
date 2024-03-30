using System;
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
		public float WayCost => path.Max(item => item.DistancePerCost);
		public float StayPoint => 0f;
		public float MovePoint => 1f;
	}
	public class StrategicPoint : ComponentBehaviour
	{
		public int pointPerTime;
		private MapPathPoint thiaPathPoint;

		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		public override void BaseAwake()
		{
			base.BaseAwake();

			thiaPathPoint = ThisContainer.GetComponent<MapPathPoint>();
		}
	}
}
