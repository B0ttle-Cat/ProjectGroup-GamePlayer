using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;


namespace BC.OdccBase
{
	public abstract class AbilityCalculationModule : ComponentBehaviour
	{
#if UNITY_EDITOR
		private void SetInfinityLifeTime()
		{
			lifeTime = float.PositiveInfinity;
		}
		[InlineButton("SetInfinityLifeTime", "Infinity")]
#endif
		public float lifeTime = float.PositiveInfinity;

		public abstract void Calculation(ref dynamic value);

		public override void BaseLateUpdate()
		{
			base.BaseLateUpdate();

			if(!UpdateLifeTime())
			{
				Destroy(this);
			}

		}
		protected virtual bool UpdateLifeTime()
		{
			if(float.IsInfinity(lifeTime)) return true;

			lifeTime -= Time.deltaTime;
			return lifeTime > 0f;
		}
	}
}
