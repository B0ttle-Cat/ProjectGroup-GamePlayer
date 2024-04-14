using BC.ODCC;

namespace BC.GamePlayerManager
{
	public interface IStartSetup : IOdccComponent
	{
		public bool IsCompleteSetting { get; }
		public void OnStartSetting();
		public void OnStopSetting();
		public void OnUpdateSetting();
	}
}
