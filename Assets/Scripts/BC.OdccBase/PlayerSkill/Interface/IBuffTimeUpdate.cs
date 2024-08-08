using BC.ODCC;

namespace BC.OdccBase
{
	public interface IBuffTimeUpdate : IOdccComponent
	{
		public double LifeTime { get; set; }
		public bool LifeTimeUpdate(in double deltaTime)
		{
			LifeTime -= deltaTime;
			bool isAlive = IsAlive;
			if(!isAlive)
			{
				OnEndLifeTime();
			}
			return isAlive;
		}
		public void OnEndLifeTime();
		public bool IsAlive { get => LifeTime > 0f; }
	}
}
