using BC.ODCC;

namespace BC.OdccBase
{
	public interface ITakeDamage : IOdccComponent
	{
		void OnTakeDamage(int value);
		void OnMissAttack();
		void OnBlockAttack();
	}
}
