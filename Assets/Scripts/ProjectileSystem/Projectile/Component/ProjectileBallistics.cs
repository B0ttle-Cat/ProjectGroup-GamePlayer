using System;

using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.ProjectileSystem
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
			if(ProjectileObject is null) return;
			UpdateBallisticTrajectory(OnHit);
		}
		public override void BaseLateUpdate()
		{
			if(ProjectileObject is null) return;
			if(CheckLateUpdateDestroy())
			{
				ProjectileObject.DestroyThis(true);
			}
		}
		protected abstract void UpdateBallisticTrajectory(Action onHit);
		protected abstract bool CheckLateUpdateDestroy();

		protected void OnHit()
		{
			if(CreateHitReport(out ProjectileHitReport hitReport))
			{
				EventManager.Call<IProjectileHitListener>(call => call.OnHit(ProjectileObject, in hitReport));
				if(CheckHitDestroy(hitReport))
				{
					ProjectileObject.DestroyThis(true);
				}
			}
		}
		protected abstract bool CreateHitReport(out ProjectileHitReport hitReport);
		protected abstract bool CheckHitDestroy(in ProjectileHitReport hitReport);
	}
}
