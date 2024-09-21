using BC.ODCC;

namespace BC.OdccBase
{
	public interface ITakeRecovery : IOdccComponent
	{
		void OnTakeRecovery(int value);
	}
}
