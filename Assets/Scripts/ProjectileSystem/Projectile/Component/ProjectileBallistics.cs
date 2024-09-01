using System;

using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.Projectile
{
	public abstract class ProjectileBallistics : ComponentBehaviour, IProjectileBallistics
	{
		public IProjectileObject ProjectileObject { get; private set; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			ProjectileObject = ThisContainer.GetObject<IProjectileObject>();
			InitBallistics();
		}
		protected abstract void InitBallistics();

		void IProjectileBallistics.OnShot(Pose onFirePose)
		{
			ThisTransform.SetWorldPose(onFirePose);
			StartBallisticTrajectory(ProjectileObject.Actor, ProjectileObject.Target);
		}
		protected abstract void StartBallisticTrajectory((ICharacterAgent agent, IUnitInteractiveValue value) actor, (ICharacterAgent agent, IUnitInteractiveValue value) target);

		public override void BaseUpdate()
		{
			UpdateBallisticTrajectory(OnHit);
		}
		protected abstract void UpdateBallisticTrajectory(Action onHit);

		// IProjectileHitListener
		protected void OnHit()
		{
			Hit();
		}
		private void Hit()
		{
			CreateHitReport(out ProjectileHitReport hitReport);

			EventManager.Call<IProjectileHitListener>(call => call.OnHit(ProjectileObject, hitReport));
		}
		protected abstract bool CreateHitReport(out ProjectileHitReport hitReport);
	}
}
