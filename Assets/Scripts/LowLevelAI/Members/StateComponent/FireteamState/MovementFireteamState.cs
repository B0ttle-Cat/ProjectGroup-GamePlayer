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
//		//private FireteamMemberCollector FireteamMembers;
//		//protected override void Disposing()
//		//{
//		//	base.Disposing();
//		//	iStateData = null;
//		//	movePathNode = null;
//		//	FireteamMembers = null;
//		//}
//		//
//		//protected override void StateEnable()
//		//{
//		//	iStateData = ThisStateData as IMovementStateData;
//		//	iStateData.IsMovement = true;
//		//
//		//	FireteamMembers = ThisContainer.GetComponent<FireteamMemberCollector>();
//		//	UpdateMove(iStateData.MovePathNode);
//		//}
//		//
//		//protected override void StateDisable()
//		//{
//		//	iStateData.IsMovement = false;
//		//
//		//	UpdateMove(null);
//		//	FireteamMembers = null;
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
//		//	if(FireteamMembers == null || FireteamMembers.Count == 0)
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
//		//		FireteamMembers.Foreach(item => {
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
//		//	if(FireteamMembers == null || FireteamMembers.Count == 0) return;
//		//
//		//	movePathNode = moveTarget;
//		//
//		//	if(movePathNode == null || movePathNode.ThisPoint == null)
//		//	{
//		//		FireteamMembers.Foreach(item => {
//		//			if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent, (agent) => agent.IsMove))
//		//			{
//		//				agent.InputAttackStop();
//		//			}
//		//		});
//		//	}
//		//	else
//		//	{
//		//
//		//		var lastNode = movePathNode.EndedNode;
//		//		var lastPrevNode = movePathNode.EndedNode.PrevNode;
//		//
//		//		Vector3 lastPosition = lastNode.ThisPoint.ThisUnitPosition();
//		//		Vector3 lastPrevPosition = lastPrevNode == null ? FireteamMembers.CenterPosition : lastPrevNode.ThisPoint.ThisUnitPosition();
//		//		Vector3 angleNormal = (lastPosition - lastPrevPosition).normalized;
//		//
//		//		Vector3 anchorPosition = lastNode.ThisPoint.ThisAnchor.ThisUnitPosition();
//		//		Vector3[] spawnAroundPoints = GetAroundPosition.GetAroundMovePosition(FireteamMembers.Count, 0f, 0f, angleNormal);
//		//
//		//		FireteamMembers.Foreach((item, index) => {
//		//			if(item.ThisContainer.TryGetComponent<FireunitMovementAgent>(out var agent))
//		//			{
//		//				agent.InputAttackTarget(movePathNode, spawnAroundPoints[index]);
//		//			}
//		//		});
//		//	}
//		//}
//	}
//}
