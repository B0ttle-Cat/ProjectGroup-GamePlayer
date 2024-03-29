using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamMovementAgent : ComponentBehaviour
	{
		public StrategicPoint moveTargetPoint;

		public void OnMovementToAnchor(int anchorIndex)
		{
			if(!ThisContainer.TryGetParentObject<ILowLevelAIManager>(out var manager)) return;

			var lowLevelAIManager = manager.LowLevelAIManager;

			if(!lowLevelAIManager.TryGetComponent<MapPathPointComputer>(out var computer)) return;

			MapPathPoint pathpoint = computer.SelectAnchorIndex(anchorIndex);
			if(pathpoint == null) return;

			if(pathpoint.ThisContainer.TryGetComponent<StrategicPoint>(out moveTargetPoint))
			{

			}
		}



	}
}
