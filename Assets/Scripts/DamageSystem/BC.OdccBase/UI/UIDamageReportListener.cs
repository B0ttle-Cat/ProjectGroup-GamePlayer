using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface UIDamageReportListener : IOdccComponent
	{
		void OnShowUIDamageItem(Vector3 showPosition, in DamageReport damageReport);
	}
}
