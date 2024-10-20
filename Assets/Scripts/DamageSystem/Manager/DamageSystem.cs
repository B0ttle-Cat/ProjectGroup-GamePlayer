using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.DamageSystem
{

	public class DamageSystem : ObjectBehaviour, IProjectileHitListener
	{

		void IProjectileHitListener.OnHit(IProjectileObject projectileObject, in ProjectileHitReport hitReport)
		{
			var hitList = hitReport.hitTargetList;
			int length = hitList.Length;
			IUnitInteractiveValue actor = projectileObject.Actor.value;
			IUnitInteractiveValue[] targetList = new IUnitInteractiveValue[length];
			for(int i = 0 ; i < length ; i++)
			{
				if(FindUnit(hitList[i], out var target))
				{
					targetList[i] = target;
				}
			}
			ComputerDamage(actor, targetList, hitReport.projectileType);
		}
		private bool FindUnit(Vector3Int memberUniqueID, out IUnitInteractiveValue find)
		{
			find = EventManager.Call<IFindCollectedMembers, IUnitInteractiveValue>(call => {
				call.TryFindFireunit(memberUniqueID, out IUnitInteractiveValue _find);
				return _find;
			});

			return find != null;
		}

		private void ComputerDamage(in IUnitInteractiveValue _actor, in IUnitInteractiveValue[] _targets, in ProjectileHitReport.ProjectileType _projectileType)
		{
			switch(_projectileType)
			{
				case ProjectileHitReport.ProjectileType.Hit_일반_공격:
				case ProjectileHitReport.ProjectileType.Hit_주력_스킬:
				case ProjectileHitReport.ProjectileType.Hit_일반_스킬:
				case ProjectileHitReport.ProjectileType.Hit_보조_스킬:
				case ProjectileHitReport.ProjectileType.Hit_특수_스킬:
					Computer(in _actor, in _targets, in _projectileType);
					break;
			}

			void Computer(in IUnitInteractiveValue actor, in IUnitInteractiveValue[] targets, in ProjectileHitReport.ProjectileType projectileType)
			{
				DamageComputer damageComputer = ThisContainer.AddComponent<DamageComputer>();
				damageComputer.OnDamageCompute(actor, targets, projectileType);
			}
		}
	}
}