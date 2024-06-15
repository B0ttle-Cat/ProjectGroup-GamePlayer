//using BC.ODCC;

//using UnityEngine;

//namespace BC.LowLevelAI
//{
//	public interface IMovementStateData : IStateData
//	{
//		public bool IsMovement { get; set; }
//		public MapPathNode MovePathNode { get; set; }
//		public bool HasMoveTarget { get; }
//	}
//	[RequireComponent(typeof(FireteamStateMachine))]
//	public class MovementFireteamState : OdccStateComponent
//	{
//		//private IMovementStateData iStateData;
//		//public MapPathNode movePathNode;
//		//
//		//private FireteamMemberCollector fireteamMembers;
//		//protected override void Disposing()
//		//{
//		//	base.Disposing();
//		//	iStateData = null;
//		//	movePathNode = null;
//		//	fireteamMembers = null;
//		//}
//		//
//		//protected override void StateEnable()
//		//{
//		//	iStateData = ThisStateData as IMovementStateData;
//		//	iStateData.IsMovement = true;
//		//
//		//	fireteamMembers = ThisContainer.GetComponent<FireteamMemberCollector>();
//		//	UpdateMove(iStateData.MovePathNode);
//		//}
//		//
//		//protected override void StateDisable()
//		//{
//		//	iStateData.IsMovement = false;
//		//
//		//	UpdateMove(null);
//		//	fireteamMembers = null;
//		//}
//		//protected override void StateChangeInHere()
//		//{
//		//	if(!iStateData.HasMoveTarget)
//		//	{
//		//		OnTransitionState<StayFireteamState>();
//		//		return;
//		//	}
//		//}
//		//protected override void StateUpdate()
//		//{
//		//	if(fireteamMembers == null || fireteamMembers.Count == 0)
//		//	{
//		//		iStateData.MovePathNode = null;
//		//		return;
//		//	}
//		//
//		//
//		//	var moveTarget = iStateData.MovePathNode;
//		//	if(movePathNode == moveTarget)
//		//	{
//		//		bool isAllStop = true;
//		//		fireteamMembers.Foreach(item => {
//		//			if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent) && agent.IsMove)
//		//			{
//		//				isAllStop = false;
//		//			}
//		//		});
//		//		if(isAllStop)
//		//		{
//		//			iStateData.MovePathNode = null;
//		//		}
//		//	}
//		//	else
//		//	{
//		//		UpdateMove(moveTarget);
//		//	}
//		//}
//		//private void UpdateMove(MapPathNode moveTarget)
//		//{
//		//	if(fireteamMembers == null || fireteamMembers.Count == 0) return;
//		//
//		//	movePathNode = moveTarget;
//		//
//		//	if(movePathNode == null || movePathNode.ThisPoint == null)
//		//	{
//		//		fireteamMembers.Foreach(item => {
//		//			if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
//		//			{
//		//				agent.InputMoveStop();
//		//			}
//		//		});
//		//	}
//		//	else
//		//	{
//		//
//		//		var lastNode = movePathNode.EndedNode;
//		//		var lastPrevNode = movePathNode.EndedNode.PrevNode;
//		//
//		//		Vector3 lastPosition = lastNode.ThisPoint.ThisPosition();
//		//		Vector3 lastPrevPosition = lastPrevNode == null ? fireteamMembers.CenterPosition : lastPrevNode.ThisPoint.ThisPosition();
//		//		Vector3 angleNormal = (lastPosition - lastPrevPosition).normalized;
//		//
//		//		Vector3 anchorPosition = lastNode.ThisPoint.ThisAnchor.ThisPosition();
//		//		Vector3[] spawnAroundPoints = GetAroundPosition.GetAroundMovePosition(fireteamMembers.Count, 0f, 0f, angleNormal);
//		//
//		//		fireteamMembers.Foreach((item, index) => {
//		//			if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
//		//			{
//		//				agent.InputMoveTarget(movePathNode, spawnAroundPoints[index]);
//		//			}
//		//		});
//		//	}
//		//}
//	}
//}
