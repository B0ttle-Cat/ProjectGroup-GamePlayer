using BC.ODCC;

namespace BC.OdccBase
{
	public interface IDamageReportListener : IOdccObject
	{
		public void OnBroadcastDamageReport(in DamageReport damageReport);
	}
}
