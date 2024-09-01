using System;

using BC.Base;
using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IGetAgentToCharacter : IOdccObject
	{
		public ICharacterToAgent ToAgent { get; set; }
	}
	public interface ICharacterAgent : IOdccComponent
	{
		public IFireunitData UnitData { get; }
		public ITransformPose TransformPose { get; }
		public IAnimation Animation { get; }
		public IWeapon Weapon { get; }

		public interface ITransformPose : IOdccComponent
		{
			public Pose WorldPose => ThisTransform.GetWorldPose();
			public Pose LocalPose => ThisTransform.GetLocalPose();
			public void OnUpdatePose(Vector3 position, Quaternion rotation);
			public void OnUpdateLocalPose(Vector3 position, Quaternion rotation);

			public Vector3 HitPosition { get; }
		}
		public interface IAnimation : IOdccComponent
		{
			public void OnUpdateMoveSpeed(float moveSpeed);
			public void OnAimTarget(bool aimTarget, Vector3 aimPos);
		}
		public interface IWeapon : IOdccComponent
		{
			public enum WeaponAttackState
			{
				None = 0,
				Aiming,
				Attack,
				Reload,
			}

			public ICharacterAgent TargetAgent { get; }
			public int AttackCount { get; }
			public int AttackMaxCount { get; }
			public WeaponAttackState AttackState { get; }

			public void OnShotAttack(ICharacterAgent targetAgent, Action<ICharacterAgent, ICharacterAgent> shotCallback);
			public void OnStopAttack();
			public void OnReload();
			public void OnImmediateReload();
			public void OnShot();


			public void OnCreateProjectile((ICharacterAgent agent, IUnitInteractiveValue value) actor, (ICharacterAgent agent, IUnitInteractiveValue value) target,
				Action<IProjectileObject, ProjectileHitReport> hitEvent);
		}
	}
}
