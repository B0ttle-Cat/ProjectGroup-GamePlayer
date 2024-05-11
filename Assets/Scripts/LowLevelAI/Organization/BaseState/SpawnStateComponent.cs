//using BC.ODCC;
//using BC.OdccBase;

//namespace BC.LowLevelAI
//{
//	public interface ISpawnStateData : IStateData
//	{
//		public MapAnchor SpawnAnchor { get; set; }
//		public bool IsWaitSpawn { get; set; }
//	}
//	public class SpawnStateComponent : OdccStateComponent
//	{
//		private ISpawnStateData iStateData;
//		private FireteamMembers fireteamMembers;
//		private SpawnData spawnData;
//		protected override void Dispose(bool disposing)
//		{
//			base.Dispose(disposing);
//			iStateData = null;
//			fireteamMembers = null;
//			spawnData = null;
//		}

//		protected override void StateAwake()
//		{
//			base.StateAwake();
//		}

//		protected override void StateEnable()
//		{
//			iStateData = ThisStateData as ISpawnStateData;
//			iStateData.IsWaitSpawn = true;

//			ThisContainer.TryGetComponent<FireteamMembers>(out fireteamMembers);
//			ThisContainer.TryGetData<SpawnData>(out spawnData);
//		}
//		protected override void StateDisable()
//		{

//		}
//		protected override void StateChangeInHere()
//		{
//			if(!iStateData.IsWaitSpawn)
//			{
//				OnTransitionState<StayStateComponent>();
//			}
//		}
//		protected override void StateUpdate()
//		{
//			if(spawnData == null && !ThisContainer.TryGetData<SpawnData>(out spawnData))
//			{
//				return;
//			}

//			if(iStateData.SpawnAnchor != null && iStateData.IsWaitSpawn)
//			{
//				var list = fireteamMembers.MemberList;
//				int count = fireteamMembers.Count;

//				SpawnUpdate(iStateData.SpawnAnchor);
//				//Vector3 position =  iStateData.SpawnAnchor.ThisPosition();
//				//
//				//ThisObject.ThisTransform.position = position;

//				iStateData.SpawnAnchor = null;
//				iStateData.IsWaitSpawn = false;
//			}
//		}



//		private void SpawnUpdate(MapAnchor target)
//		{
//			//Vector3[] formaitionPosition = target.GetAroundMovePosition(fireteamMembers.Count, angleNormal);

//			//	fireteamMembers.Foreach((item, index) => {
//			//		if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
//			//		{
//			//			agent.InputMoveTarget(movePathNode, formaitionPosition[index]);
//			//		}
//			//	});
//		}
//	}
//}
