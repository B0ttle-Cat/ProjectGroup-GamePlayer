//using BC.ODCC;

//namespace BC.LowLevelAI
//{
//	public class FireteamStateMachine : OdccFiniteStateMachine
//	{
//		private FireteamStateData stateData;
//		sealed public override OdccStateData ThisStateData => stateData;

//		public override void BaseValidate()
//		{
//			base.BaseValidate();
//			if(!ThisContainer.TryGetData<FireteamStateData>(out stateData))
//			{
//				stateData = ThisContainer.AddData<FireteamStateData>();
//			}
//		}
//		protected override void FSMAwake()
//		{
//			if(!ThisContainer.TryGetData<FireteamStateData>(out stateData))
//			{
//				stateData = ThisContainer.AddData<FireteamStateData>();
//			}
//		}
//		protected override void FSMDestroy()
//		{
//			stateData = null;
//		}

//		//public void OnTeamCommand_SetMoveTarget(IGetLowLevelAIManager manager, int anchorIndex)
//		//{
//		//	if(manager == null || manager.LowLevelAI == null) return;

//		//	if(!manager.MapStage.ThisContainer.TryGetComponent<MapPathPointComputer>(out var computer)) return;

//		//	if(!ThisContainer.TryGetComponent<FireteamMemberCollector>(out var members)) return;

//		//	Vector3 center = members.CenterPosition;

//		//	if(!computer.TryGetClosedPathPoint(center, out var closedPathPoint)) return;

//		//	var moveTargetPoint = computer.TrySelectPathPointIndex(anchorIndex);
//		//	if(closedPathPoint ==null || moveTargetPoint == null) return;

//		//	if(!closedPathPoint.CalculatePath(moveTargetPoint, out var pathNode)) return;
//		//	stateData.MovePathNode = pathNode;
//		//}

//		//public void OnTeamCommand_SpawnOnAnchor(IGetLowLevelAIManager manager, int anchorIndex)
//		//{
//		//	if(manager == null || manager.LowLevelAI == null) return;

//		//	if(!manager.MapStage.ThisContainer.TryGetComponent<MapPathPointComputer>(out var computer)) return;

//		//	var teleportationAnchor = computer.TrySelectAnchorIndex(anchorIndex);
//		//	if(teleportationAnchor == null) return;

//		//	if(!ThisContainer.TryGetComponent<FireteamMemberCollector>(out var members)) return;
//		//	int index = 0;
//		//	members.Foreach(unitObject => {
//		//		var spawn = new SpawnData() {
//		//			targetAnchor = teleportationAnchor,
//		//			totalUnitCount = members.Count,
//		//			unitIndex = index++,
//		//			targetRadius = 2f,
//		//		};
//		//		unitObject.ThisContainer.RemoveData<SpawnData>();
//		//		unitObject.ThisContainer.RemoveComponent<SpawnComponent>();

//		//		unitObject.ThisContainer.AddData<SpawnData>(spawn);
//		//		unitObject.ThisContainer.AddComponent<SpawnComponent>();
//		//	});
//		//}
//	}
//}
