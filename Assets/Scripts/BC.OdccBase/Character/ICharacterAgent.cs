using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IGetAgentToCharacter : IOdccObject
	{
		public ICharacterToAgent ToAgent { get; set; }
	}
	public interface ICharacterAgent : IOdccComponent
	{
		public interface Agent : IOdccComponent
		{
			public ICharacterToAgent ToAgent { get { return ThisContainer.GetObject<IGetAgentToCharacter>()?.ToAgent; } }
			public IFireunitData UnitData { get { return ThisContainer.GetData<IFireunitData>(); } }
		}
		public interface TransformPose : IOdccComponent
		{
			public void OnUpdatePose(Vector3 position, Quaternion rotation);
			public void OnUpdateLocalPose(Vector3 position, Quaternion rotation);
		}
		public interface Scale : IOdccComponent
		{
			public void OnUpdateScale(Vector3 scale);
		}
		public interface Velocity : IOdccComponent
		{
			public void OnUpdateVelocity(Vector3 velocity);
		}
		public interface MoveSpeed : IOdccComponent
		{
			public void OnUpdateMoveSpeed(float moveSpeed);
		}
		public interface IsAimTarget : IOdccComponent
		{
			public void OnAimTarget(bool aimTarget);
		}
	}
}
