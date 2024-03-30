using BC.ODCC;

namespace BC.LowLevelAI
{
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
