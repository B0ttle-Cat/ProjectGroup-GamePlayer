using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public interface ITeamInteractiveValue : IOdccComponent
	{
		public IFireteamData ThisTeamData { get; set; }

		public Vector3 ThisTeamPosition { get; set; }


		void OnUpdateInit();
		void OnUpdateValue();
	}
}
