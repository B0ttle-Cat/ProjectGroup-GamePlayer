using System;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.ProjectileSystem
{
	public class ProjectileSystem : ObjectBehaviour, IProjectileSystem
	{
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
	}
}
