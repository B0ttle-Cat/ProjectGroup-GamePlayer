namespace BC.GamePlayerInterface
{
	//public class LowLevelPlayingInterface : ComponentBehaviour
	//{
	//	private ILowLevelPlayingInterface interfaceReceiver;

	//	private bool IsValid => interfaceReceiver != null;
	//	private bool IsNotValid => !IsValid;

	//	public override void BaseEnable()
	//	{
	//		base.BaseEnable();
	//		interfaceReceiver = ThisContainer.GetComponent<ILowLevelPlayingInterface>();

	//	}


	//	[InlineButton("OnPlayingInterface_SelectFireteam")]
	//	public int selectTeamIndex;
	//	public void OnLowPlayingInterface_SelectFireteam()
	//	{
	//		if(IsNotValid) return;
	//		interfaceReceiver.OnPlayingInterface_SelectFireteam(selectTeamIndex);
	//	}

	//	[InlineButton("OnTeamCommand_SetMoveTarget")]
	//	public int movementToAnchor;
	//	public void OnPlayingInterface_SetMoveTarget()
	//	{
	//		if(IsNotValid) return;
	//		if(selectTeamIndex < 0) selectTeamIndex = interfaceReceiver.OnPlayingInterface_GetSelectedFireteam();
	//		interfaceReceiver.OnPlayingInterface_SetMoveTarget(movementToAnchor, selectTeamIndex);
	//	}

	//	[InlineButton("OnPlayingInterface_SpawnTeamToAnchor")]
	//	public int teamSpawnToAnchor;
	//	public void OnPlayingInterface_SpawnTeamToAnchor()
	//	{
	//		if(IsNotValid) return;
	//		if(selectTeamIndex < 0) selectTeamIndex = interfaceReceiver.OnPlayingInterface_GetSelectedFireteam();
	//		interfaceReceiver.OnPlayingInterface_SpawnTeamToAnchor(teamSpawnToAnchor, selectTeamIndex);
	//	}
	//}
}
