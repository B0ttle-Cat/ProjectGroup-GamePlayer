using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamMovementAgent : ComponentBehaviour
	{
		public StrategicPoint moveTargetPoint;

		public void OnMovementToAnchor(int anchorIndex)
		{
			if(!ThisContainer.TryGetParentObject<LowLevelAIManager>(out var manager)) return;

			if(!manager.ThisContainer.TryGetComponent<MapWaypointComputer>(out var computer)) return;

			MapWaypoint wayPoint = computer.SelectAnchorIndex(anchorIndex);
			if(wayPoint == null) return;

			if(wayPoint.ThisContainer.TryGetComponent<StrategicPoint>(out moveTargetPoint))
			{

			}
		}



	}
}
