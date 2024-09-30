using BC.ODCC;

namespace BC.OdccBase
{
	public interface IStartSetup : IOdccComponent
	{
		public bool IsCompleteSetting { get; }
	}
}
