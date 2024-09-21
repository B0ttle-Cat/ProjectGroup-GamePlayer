using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class FireunitInteractiveValue : ITakeRecovery, ITakeDamage//.Damage : MonoBehaviour
	{
		void ITakeDamage.OnTakeDamage(int value)
		{
			var healthPoint = PlayValueData.HealthPoint;
			healthPoint.Value -= value;
			PlayValueData.HealthPoint = healthPoint;
		}
		void ITakeRecovery.OnTakeRecovery(int value)
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
