using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class FireunitInteractiveValue : ITakeRecovery, ITakeDamage//.Damage : MonoBehaviour
	{
		void ITakeDamage.OnTakeDamage(in int value)
		{
			var healthPoint = PlayValueData.ä��;
			healthPoint.Value -= value;
			PlayValueData.ä�� = healthPoint;
		}
		void ITakeRecovery.OnTakeRecovery(in int value)
		{
			var healthPoint = PlayValueData.ä��;
			healthPoint.Value += value;
			PlayValueData.ä�� = healthPoint;
		}
		void ITakeDamage.OnMissAttack()
		{

		}
		void ITakeDamage.OnBlockAttack()
		{

		}
	}
}
