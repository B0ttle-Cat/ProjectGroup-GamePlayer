using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class FireunitInteractiveValue : ITakeRecovery, ITakeDamage//.Damage : MonoBehaviour
	{
		void ITakeDamage.OnTakeDamage(in int value)
		{
			var healthPoint = PlayValueData.HealthPoint;
			healthPoint.Value -= value;
			PlayValueData.HealthPoint = healthPoint;
		}
		void ITakeRecovery.OnTakeRecovery(in int value)
		{
			var healthPoint = PlayValueData.HealthPoint;
			healthPoint.Value += value;
			PlayValueData.HealthPoint = healthPoint;
		}
		void ITakeDamage.OnMissAttack()
		{

		}
		void ITakeDamage.OnBlockAttack()
		{

		}
	}
}
