using BC.ODCC;

namespace BC.OdccBase
{
	public interface IBuffTimeUpdate : IOdccComponent
	{
		public double LastUpdateTime { get; set; }
		public double LifeTime { get; set; }
		public bool LifeTimeUpdate()
		{
			if(IsAlive)
			{
				double deltaTime = UnityEngine.Time.timeAsDouble - LastUpdateTime;
				LastUpdateTime = UnityEngine.Time.timeAsDouble;
				LifeTime -= deltaTime;
				bool isAlive = IsAlive;
				if(!isAlive)
				{
					OnEndLifeTime();
				}
				return isAlive;
			}
			else
			{
				return false;
			}
		}
		public void OnEndLifeTime();
		public bool IsAlive { get => LifeTime > 0f; }
	}
}
