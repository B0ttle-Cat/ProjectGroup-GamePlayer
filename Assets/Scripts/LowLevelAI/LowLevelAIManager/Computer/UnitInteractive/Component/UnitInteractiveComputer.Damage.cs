using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class UnitInteractiveComputer : IDamageReportListener //.Damage
	{
		void IDamageReportListener.OnBroadcastDamageReport(DamageReport damageReport)
		{
			var actorID = damageReport.Actor;
			var targetID = damageReport.Target;

			if(!(unitInteractiveValueList.TryGetValue(actorID, out var actor) && unitInteractiveValueList.TryGetValue(targetID, out var target)))
			{
				return;
			}
			// TODO : damageReport �� ����� ü�� ����

			//actor.
			var damageType = damageReport.damageType;
			var subDamageType = damageReport.subDamageType;

			switch(damageType)
			{
				case DamageReport.DamageType.����: TakeDamage(); break;
				case DamageReport.DamageType.ȸ��: TakeRecovery(); break;
				case DamageReport.DamageType.������: MissAttack(); break;
				case DamageReport.DamageType.���ܵ�: BlockAttack(); break;
			}

			void TakeDamage()
			{
				target.TakeDamage.OnTakeDamage(damageReport.value);
			}
			void TakeRecovery()
			{
				//actor.HealthPoint += damageReport.value,
				target.TakeRecovery.OnTakeRecovery(damageReport.value);
			}
			void MissAttack()
			{
				target.TakeDamage.OnMissAttack();
			}
			void BlockAttack()
			{
				target.TakeDamage.OnBlockAttack();
			}
		}
	}
}
