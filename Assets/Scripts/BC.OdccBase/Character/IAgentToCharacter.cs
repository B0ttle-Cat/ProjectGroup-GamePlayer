using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IGetAgentToCharacter : IOdccObject
	{
		public ICharacterToAgent ToAgent { get; set; }
	}
	public interface IAgentToCharacter : IOdccComponent
	{
		public ICharacterToAgent ToAgent { get { return ThisContainer.GetObject<IGetAgentToCharacter>()?.ToAgent; } }
		public IFireunitData UnitData { get { return ThisContainer.GetData<IFireunitData>(); } }

		public void OnUpdatePose(Vector3 position, Quaternion rotation);
		public void OnUpdateMovment(Vector3 velocity);
	}
}
