using BC.Base;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.DamageSystem
{

	public class DamageManager : SingletonObjectBehaviour<DamageManager>, IProjectileHitListener
	{
		protected override void CreatedSingleton()
		{
		}

		protected override void DestroySingleton()
		{
		}

		void IProjectileHitListener.OnHit(IProjectileObject _projectileObject, in ProjectileHitReport _hitReport)
		{

			if(_hitReport.IsHit) Hit(in _projectileObject, in _hitReport);
			else if(_hitReport.IsMiss) Miss(in _projectileObject, in _hitReport);

			void Hit(in IProjectileObject projectileObject, in ProjectileHitReport hitReport)
			{
				var hitList = hitReport.HitTargetList;
				int length = hitList.Length;
				for(int i = 0 ; i < length ; i++)
				{
					if(FindUnit(hitList[i], out var target))
					{
						var actor = projectileObject.Actor.value;
						GiveAndTakeDamage(actor, target, hitReport.IsHitType);
					}
				}
			}
			void Miss(in IProjectileObject projectileObject, in ProjectileHitReport hitReport)
			{
				var hitList = hitReport.HitTargetList;
				int length = hitList.Length;
				for(int i = 0 ; i < length ; i++)
				{
					if(FindUnit(hitList[i], out var target))
					{
						var actor = projectileObject.Actor.value;
						GiveAndTakeDamage(actor, target, hitReport.IsHitType);
					}
				}
			}
		}
		private bool FindUnit(Vector3Int memberUniqueID, out IUnitInteractiveValue find)
		{
			find = EventManager.Call<IFindCollectedMembers, IUnitInteractiveValue>(call => {
				call.TryFindFireunit(memberUniqueID, out IUnitInteractiveValue _find);
				return _find;
			});

			return find != null;
		}

		private void GiveAndTakeDamage(IUnitInteractiveValue giver, IUnitInteractiveValue taker, ProjectileHitReport.HitType hitType)
		{
			// TODO:: ���⼭ ������ ����ϴ� �溾�� ����ʿ���.
			switch(hitType)
			{
				case ProjectileHitReport.HitType.None:
					break;

				case ProjectileHitReport.HitType.Hit:
					break;
				case ProjectileHitReport.HitType.Hit_�Ϲ�_����:
					break;
				case ProjectileHitReport.HitType.Hit_�Ϲ�_��ų:
					break;

				case ProjectileHitReport.HitType.Miss:
					break;
				case ProjectileHitReport.HitType.Miss_����ü��_����������_���:
					break;
				case ProjectileHitReport.HitType.Miss_��ǥ��_�̹�_����ȭ��:
					break;
				case ProjectileHitReport.HitType.Miss_��ǥ��_�Ҿ����:
					break;
				case ProjectileHitReport.HitType.Miss_�����_�̹�_����ȭ��:
					break;
				case ProjectileHitReport.HitType.Miss_�����_�Ͼ����:
					break;
				default:
					break;
			}
		}
	}
}