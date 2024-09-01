using System;

using BC.OdccBase;

using UnityEngine;

namespace BC.Projectile
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
			float time = 1 - (projectileTime / maxLifeTime);
			if(time >= 1f)
			{
				time = 1f;
				currentPos = Vector3.Lerp(startPos, targetPos, time);
				onHit?.Invoke();
				enabled = false;
			}
			else
			{
				currentPos = Vector3.Lerp(startPos, targetPos, time);
			}
		}

		protected override bool CreateHitReport(out ProjectileHitReport hitReport)
		{
			(ICharacterAgent actorAgent, IUnitInteractiveValue actorValue) = actor;
			(ICharacterAgent targetAgent, IUnitInteractiveValue targetValue) = target;

			if(targetValue.ThisObject == null)
			{
				hitReport = new ProjectileHitReport(actor, ProjectileHitReport.HitType.Miss_타겟을_잃어버림);
			}
			else if(targetValue.StateValueData.IsRetire)
			{
				hitReport = new ProjectileHitReport(actor, ProjectileHitReport.HitType.Miss_타겟이_이미_무력화됨);
			}
			else if(actorAgent.ThisObject == null)
			{
				hitReport = new ProjectileHitReport(actor, ProjectileHitReport.HitType.Miss_발주를_일어버림);
			}
			else if(actorValue.StateValueData.IsRetire)
			{
				hitReport = new ProjectileHitReport(actor, ProjectileHitReport.HitType.Miss_발주가_이미_무력화됨);
			}
			else
			{
				hitReport = new ProjectileHitReport(actor, ProjectileHitReport.HitType.Hit_일반_공격, new int[1] {
					target.value.MemberUniqueID
				});
			}
			return true;
		}
	}
}
