using BC.ODCC;

namespace BC.OdccBase
{
	public interface IStartSetup : IOdccComponent
	{
		public bool IsCompleteSetting { get; set; }
		public void OnStartSetting()
		{

		}
		public void OnStopSetting()
		{

		}
		public void OnUpdateSetting()
		{

		}
	}
}
