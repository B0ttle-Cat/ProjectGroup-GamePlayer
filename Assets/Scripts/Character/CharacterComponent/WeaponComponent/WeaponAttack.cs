using System;

using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.Character
{
	public class WeaponAttack : ComponentBehaviour, ICharacterAgent.IWeapon
	{
		protected WeaponModel model;

		private ICharacterAgent thisCharacterAgent;
		public CharacterData CharacterData { get; set; }
		public WeaponData WeaponData { get; set; }
		public ICharacterAgent TargetAgent { get; protected set; }
		public Action<ICharacterAgent, ICharacterAgent> ShotCallback { get; protected set; }
		public int AttackCount { get; protected set; }
		public int AttackMaxCount { get; protected set; }
		public ICharacterAgent.IWeapon.WeaponAttackState AttackState { get; protected set; }

		private ICharacterAgent targetAgent;
		private Action<ICharacterAgent, ICharacterAgent> shotCallback;

		public override void BaseAwake()
		{
			base.BaseAwake();
			CharacterData = null;
			WeaponData = null;
			AttackState = ICharacterAgent.IWeapon.WeaponAttackState.None;

			model = ThisContainer.GetComponent<WeaponModel>();
			ThisContainer.NextGetComponent<ICharacterAgent>(c => thisCharacterAgent=c);
			ThisContainer.NextGetData<CharacterData>((characterData) => {
				ThisContainer.NextGetData<WeaponData>((weaponData) => {
					CharacterData = characterData;
					WeaponData = weaponData;
					AttackMaxCount = weaponData.MaxAttackCount;
					AttackCount = AttackMaxCount;
				});
			});
		}

		public void OnShotAttack(ICharacterAgent targetAgent, Action<ICharacterAgent, ICharacterAgent> shotCallback)
		{
			if(CharacterData == null || WeaponData == null) return;
			TargetAgent = targetAgent;
			ShotCallback = shotCallback;
		}
		public void OnStopAttack()
		{
			if(CharacterData == null || WeaponData == null) return;
			TargetAgent = null;
			ShotCallback = null;
		}
		public void OnReload()
		{
			if(CharacterData == null || WeaponData == null) return;
		}
		public void OnImmediateReload()
		{
			if(CharacterData == null || WeaponData == null) return;
		}
		public void OnShot()
		{
			var target = targetAgent ?? TargetAgent;
			if(thisCharacterAgent != null && target != null)
			{
				shotCallback?.Invoke(thisCharacterAgent, target);
			}
		}
		void ICharacterAgent.IWeapon.OnCreateProjectile((ICharacterAgent agent, IUnitInteractiveValue value) actor, (ICharacterAgent agent, IUnitInteractiveValue value) target, Action<IProjectileObject, ProjectileHitReport> hitEvent)
		{
			if(model.muzzlePivot == null) return;
			if(model.bulletPrefab == null) return;
			if(model.bulletPrefab.TryGetComponent<ObjectBehaviour>(out var prefabObj))
			{
				Pose onFirePose = model.muzzlePivot.GetWorldPose();
				EventManager.Call<IProjectileSystem>(call => call.OnCreateProjectile(prefabObj, onFirePose, actor, target));
			}
		}


		public override void BaseStart()
		{

			base.BaseStart();
			WeaponAttackStateUpdate();
		}
		protected virtual async void WeaponAttackStateUpdate()
		{
			while(true)
			{
				await Awaitable.NextFrameAsync(DestroyCancelToken);
				if(!isActiveAndEnabled) continue;
				if(CharacterData == null) continue;

				switch(AttackState)
				{
					case ICharacterAgent.IWeapon.WeaponAttackState.None:
						if(AttackCount<=0) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Reload;
						else if(TargetAgent != null) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Aiming;
						else AttackState = ICharacterAgent.IWeapon.WeaponAttackState.None;
						break;
					case ICharacterAgent.IWeapon.WeaponAttackState.Aiming:
						if(AttackCount<=0) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Reload;
						else if(TargetAgent != null) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Attack;
						else AttackState = ICharacterAgent.IWeapon.WeaponAttackState.None;
						break;
					case ICharacterAgent.IWeapon.WeaponAttackState.Attack:
						if(AttackCount<=0) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Reload;
						else if(TargetAgent != null) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Attack;
						else AttackState = ICharacterAgent.IWeapon.WeaponAttackState.None;
						break;
					case ICharacterAgent.IWeapon.WeaponAttackState.Reload:
						if(AttackCount<=0) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Reload;
						else if(TargetAgent != null) AttackState = ICharacterAgent.IWeapon.WeaponAttackState.Aiming;
						else AttackState = ICharacterAgent.IWeapon.WeaponAttackState.None;
						break;
				}

				switch(AttackState)
				{
					case ICharacterAgent.IWeapon.WeaponAttackState.None:
						await NoneUpdate();
						break;
					case ICharacterAgent.IWeapon.WeaponAttackState.Aiming:
						await AimingUpdate();
						break;
					case ICharacterAgent.IWeapon.WeaponAttackState.Attack:
						targetAgent = TargetAgent;
						shotCallback = ShotCallback;
						await AttackUpdate(OnShot);
						if(AttackCount>0) await AttackDelayUpdate();
						targetAgent = null;
						shotCallback = null;
						break;
					case ICharacterAgent.IWeapon.WeaponAttackState.Reload:
						await ReloadUpdate();
						await ReloadDelayUpdate();
						break;
				}
			}
		}

		protected virtual async Awaitable NoneUpdate()
		{

		}
		protected virtual async Awaitable AimingUpdate()
		{

		}
		protected virtual async Awaitable AttackUpdate(Action shotEvent)
		{
			shotEvent?.Invoke();
		}
		protected virtual async Awaitable AttackDelayUpdate()
		{

		}
		protected virtual async Awaitable ReloadUpdate()
		{

		}
		protected virtual async Awaitable ReloadDelayUpdate()
		{

		}
	}
}
