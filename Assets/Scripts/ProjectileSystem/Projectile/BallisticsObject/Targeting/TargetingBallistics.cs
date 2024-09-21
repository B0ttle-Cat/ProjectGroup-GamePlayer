using System;

using BC.OdccBase;

using UnityEngine;

namespace BC.ProjectileSystem
{
	public class TargetingBallistics : ProjectileBallistics
	{
		TargetingBallisticsData targetingBallisticsData;

		Vector3 startPos;
		Vector3 targetPos;

		Vector3 currentPos;

		(ICharacterAgent agent, IUnitInteractiveValue value) actor;
		(ICharacterAgent agent, IUnitInteractiveValue value) target;

		private float projectileTime;
		private float maxLifeTime;

		public override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<TargetingBallisticsData>(out _))
			{
				ThisContainer.AddData<TargetingBallisticsData>();
			}
		}

		protected override void InitBallistics()
		{
			ThisContainer.TryGetData<TargetingBallisticsData>(out targetingBallisticsData);
		}


		protected override void StartBallisticTrajectory((ICharacterAgent agent, IUnitInteractiveValue value) actor, (ICharacterAgent agent, IUnitInteractiveValue value) target)
		{
			if(targetingBallisticsData == null) return;

			this.actor = actor;
			this.target = target;

			startPos = ThisTransform.position;
			currentPos = startPos;
			targetPos = target.agent.TransformPose.HitPosition;

			float endPointDistance = Vector3.Distance(startPos, targetPos);

			if(targetingBallisticsData.projectileSpeed <= 0)
				targetingBallisticsData.projectileSpeed = 1f;

			maxLifeTime = (endPointDistance / targetingBallisticsData.projectileSpeed);
			projectileTime = maxLifeTime;
			enabled = true;
		}

		protected override void UpdateBallisticTrajectory(Action onHit)
		{
			if(targetingBallisticsData == null) return;

			(ICharacterAgent targetAgent, IUnitInteractiveValue targetValue) = target;
			if(targetAgent != null)
			{
				targetPos = targetAgent.TransformPose.HitPosition;
			}


			projectileTime -= Time.deltaTime;
			float lerpProgress = 1f - (projectileTime / maxLifeTime);
			if(lerpProgress <= 1f)
			{
				lerpProgress = 1f;
				currentPos = Vector3.Lerp(startPos, targetPos, lerpProgress);
				onHit?.Invoke();
				enabled = false;
			}
			else
			{
				currentPos = Vector3.Lerp(startPos, targetPos, lerpProgress);
			}
		}
		protected override bool CheckLateUpdateDestroy()
		{
			return projectileTime < -0.5;
		}

		protected override bool CreateHitReport(out ProjectileHitReport hitReport)
		{
			(ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) = actor;
			(ICharacterAgent targetAgent, IUnitInteractiveValue targetValue) = target;

			hitReport = new ProjectileHitReport(actor, ProjectileHitReport.ProjectileType.Hit_일반_공격, ProjectileHitReport.SubProjectileType.미분류, new Vector3Int[1] {
				target.value.MemberUniqueID
			});
			return true;
		}

		protected override bool CheckHitDestroy(in ProjectileHitReport hitReport)
		{
			return true;
		}
	}
}
