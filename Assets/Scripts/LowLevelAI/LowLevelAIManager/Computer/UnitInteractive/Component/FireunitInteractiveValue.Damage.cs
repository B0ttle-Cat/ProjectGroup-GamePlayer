using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class FireunitInteractiveValue : ITakeRecovery, ITakeDamage//.Damage : MonoBehaviour
	{
		void ITakeDamage.OnTakeDamage(in int value)
		{
			var healthPoint = PlayValueData.채력;
			healthPoint.Value -= value;
			PlayValueData.채력 = healthPoint;
		}
		void ITakeRecovery.OnTakeRecovery(in int value)
		{
			var healthPoint = PlayValueData.채력;
			healthPoint.Value += value;
			PlayValueData.채력 = healthPoint;
		}
		void ITakeDamage.OnMissAttack()
		{

		}
		void ITakeDamage.OnBlockAttack()
		{

		}
	}
}
