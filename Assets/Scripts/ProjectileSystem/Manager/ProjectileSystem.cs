using System;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.Projectile
{
	public class ProjectileSystem : SingletonObjectBehaviour<ProjectileSystem>, IProjectileSystem, IProjectileHitListener
	{
		protected override void CreatedSingleton()
		{
		}

		protected override void DestroySingleton()
		{
		}

		async void IProjectileSystem.OnCreateProjectile(ObjectBehaviour projectilePrefab, Pose onFirePose,
			(ICharacterAgent agent, IUnitInteractiveValue value) actor,
			(ICharacterAgent agent, IUnitInteractiveValue value) target)
		{
			if(projectilePrefab == null) return;
			if(projectilePrefab is not ProjectileObject)
			{
				Debug.LogError("ProjectileSystem.OnCreateProjectile: projectilePrefab does not have IProjectileObject component.");
				return;
			}
			if(projectilePrefab.gameObject.activeSelf)
			{
				projectilePrefab.gameObject.SetActive(false);
			}
			AsyncInstantiateOperation<ObjectBehaviour> waitObject = GameObject.InstantiateAsync<ObjectBehaviour>(projectilePrefab);
			await Awaitable.FromAsyncOperation(waitObject);
			ProjectileObject newProjectile = null;
			try
			{
				ObjectBehaviour newObject = waitObject.Result[0];
				if(newObject is ProjectileObject _newProjectile)
				{
					newProjectile = _newProjectile;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
			if(newProjectile == null) return;


			newProjectile.gameObject.SetActive(true);
			IProjectileObject iProjectileObject = newProjectile.ThisContainer.GetObject<IProjectileObject>();
			iProjectileObject.Actor = actor;
			iProjectileObject.Target = target;
			iProjectileObject.Ballistics.OnShot(onFirePose);
		}


		void IProjectileHitListener.OnHit(IProjectileObject projectileObject, ProjectileHitReport hitReport)
		{
			// TODO:: ���� ���⼭ Hit �� ĳ���͸� ã�Ƽ� ���ظ� �� ���� ��.

			projectileObject.DestroyThis(true);
		}
	}
}
