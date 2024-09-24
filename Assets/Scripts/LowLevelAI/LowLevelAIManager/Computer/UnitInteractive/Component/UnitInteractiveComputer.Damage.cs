using BC.Base;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class UnitInteractiveComputer : IDamageReportListener //.Damage
	{
		void IDamageReportListener.OnBroadcastDamageReport(in DamageReport _damageReport)
		{
			DamageReport damageReport = _damageReport;
			var actorID = damageReport.Actor;
			var targetID = damageReport.Target;

			if(!(unitInteractiveValueList.TryGetValue(actorID, out var actor) && unitInteractiveValueList.TryGetValue(targetID, out var target)))
			{
				return;
			}
			// TODO : damageReport �� ����� ü�� ����

			var damageType = damageReport.damageType;
			var subDamageType = damageReport.subDamageType;

			int value = damageReport.value;
			Vector3 uiPosition = target.PoseValueData.ThisDamageUIPosition;
			string showDamageText = "";
			switch(damageType)
			{
				case DamageReport.DamageType.����: TakeDamage(in value, ref showDamageText); break;
				case DamageReport.DamageType.ȸ��: TakeRecovery(in value, ref showDamageText); break;
				case DamageReport.DamageType.������: MissAttack(ref showDamageText); break;
				case DamageReport.DamageType.���ܵ�: BlockAttack(ref showDamageText); break;
			}

			EventManager.Call<UIDamageReportListener>(call => call.OnShowUIDamageItem(uiPosition, in damageReport));

			void TakeDamage(in int _value, ref string message)
			{
				target.TakeDamage.OnTakeDamage(in _value);
				message = $"{_value}";
			}
			void TakeRecovery(in int _value, ref string message)
			{
				target.TakeRecovery.OnTakeRecovery(in _value);
				message = $"{_value}";
			}
			void MissAttack(ref string message)
			{
				target.TakeDamage.OnMissAttack();
				message = "Miss";
			}
			void BlockAttack(ref string message)
			{
				target.TakeDamage.OnBlockAttack();
				message = "Block";
			}
		}
	}
}
