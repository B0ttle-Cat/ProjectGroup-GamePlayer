using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IProjectileBallistics : IOdccComponent
	{
		void OnShot(Pose onFirePose);
	}
}
