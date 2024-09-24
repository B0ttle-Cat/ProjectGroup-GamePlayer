using BC.ODCC;

namespace BC.OdccBase
{
	public interface ITakeRecovery : IOdccComponent
	{
		void OnTakeRecovery(in int value);
	}
}
