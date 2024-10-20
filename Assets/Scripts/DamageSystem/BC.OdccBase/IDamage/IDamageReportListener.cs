using BC.ODCC;

namespace BC.OdccBase
{
	public interface IDamageReportListener : IOdccComponent
	{
		public void OnBroadcastDamageReport(in DamageReport damageReport);
	}
}
