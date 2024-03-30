using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

namespace BC.LowLevelAI
{
	public interface IFireteamMovement
	{
		public void InputMoveTarget(Vector3 target);
	}
	public interface IFireunitMovement
	{
		public void InputMoveTarget(Vector3 target);
	}

	public class FireunitMovementAgent : ComponentBehaviour
	{
		protected NavMeshAgent navMeshAgent;
		protected NavMeshObstacle navMeshObstacle;
		[ShowInInspector, ReadOnly]
		protected Vector3 inputTarget;

		protected float baseRadius;
		protected float halfRadius;

		[ShowInInspector, ReadOnly]
		private NavMeshPathStatus navMeshPathStatus;
		private NavMeshPath navMeshPath;
		public bool IsMove;

		public override void BaseAwake()
		{
			base.BaseAwake();
			InitAgent();
		}
		public virtual void InitAgent()
		{
			navMeshAgent = ThisObject.GetComponentInChildren<NavMeshAgent>();
			if(navMeshAgent == null)
			{
				navMeshAgent = null;
				return;
			}

			navMeshObstacle = ThisObject.GetComponentInChildren<NavMeshObstacle>();
			if(navMeshObstacle == null)
			{
				navMeshObstacle = null;
			}

			baseRadius = navMeshAgent.radius;
			halfRadius = baseRadius * 0.5f;

			navMeshAgent.stoppingDistance = halfRadius;

			navMeshPath = new NavMeshPath();
		}

		public void InputMoveStop(Vector3? stopWarp = null)
		{
			if(navMeshAgent is null || navMeshPath is null) return;

			navMeshAgent.ResetPath();
			if(stopWarp.HasValue)
			{
				navMeshAgent.Warp(stopWarp.Value);
			}
			navMeshAgent.isStopped = true;
			IsMove = false;
		}
		public void InputMoveTarget(Vector3 target, bool isWarp = false)
		{
			if(navMeshAgent is null || navMeshPath is null) return;
			if(NavMesh.SamplePosition(target, out var navMeshHit, 10f, NavMesh.AllAreas))
			{
				target = navMeshHit.position;
			}

			inputTarget = target;
			if(isWarp)
			{
				navMeshAgent.ResetPath();
				navMeshAgent.Warp(inputTarget);
				navMeshAgent.isStopped = true;
				IsMove = false;
				return;
			}

			navMeshAgent.isStopped = false;
			IsMove = true;

			if(navMeshAgent.CalculatePath(inputTarget, navMeshPath))
			{
				if(navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
				{
					navMeshAgent.SetDestination(inputTarget);
					navMeshPath = navMeshAgent.path;
				}
				else
				{
					navMeshAgent.SetPath(navMeshPath);
				}
			}
			else
			{
				navMeshAgent.SetDestination(inputTarget);
				navMeshPath = navMeshAgent.path;
			}
		}

		public override void BaseUpdate()
		{
			base.BaseUpdate();
			if(navMeshAgent is null || navMeshPath is null || !navMeshAgent.isActiveAndEnabled)
			{
				return;
			}
			if(!IsMove) return;


			navMeshPathStatus = navMeshAgent.pathStatus;
			bool hasPath = navMeshAgent.hasPath;
			if(!hasPath)
			{
				return;
			}
			bool isPathStale = navMeshAgent.isPathStale;
			bool pathPending = navMeshAgent.pathPending;

			//if(isPathStale)
			//{
			//	InputMoveTarget(inputTarget);
			//	return;
			//}

			float remainingDistance = navMeshAgent.remainingDistance;
			float stoppingDistance = navMeshAgent.stoppingDistance;
			float radius = navMeshAgent.radius;

			if(navMeshPathStatus == NavMeshPathStatus.PathComplete)
			{
				// 경로를 올바르게 찾음 
			}
			else if(pathPending)
			{
				Vector3 diraction = Vector3.zero;
				Vector3 currentPos = navMeshAgent.nextPosition;
				var corners = navMeshPath.corners;
				if(corners.Length < 2)
				{
					diraction = currentPos - inputTarget;
				}
				else if(corners.Length == 2)
				{
					diraction = currentPos - corners[1];
				}
				else
				{
					Vector3 distance1 = corners[1] - corners[2];
					Vector3 distance2 = currentPos - corners[2];
					if(distance1.sqrMagnitude < distance2.sqrMagnitude)
					{
						diraction = currentPos - corners[1];
					}
					else
					{
						diraction = distance2;
					}
				}


				float velocity = navMeshAgent.velocity.magnitude;
				velocity += navMeshAgent.acceleration * Time.deltaTime;

				float speed = navMeshAgent.speed;
				if(velocity > speed)
				{
					velocity = speed;
				}

				navMeshAgent.Move(diraction.normalized * velocity);
			}
			else if(navMeshPathStatus == NavMeshPathStatus.PathPartial || navMeshPathStatus == NavMeshPathStatus.PathInvalid)
			{
				navMeshAgent.SetDestination(inputTarget);
				navMeshPath = navMeshAgent.path;
			}


			if(IsMove && remainingDistance <= stoppingDistance)
			{
				IsMove = false;
				navMeshAgent.isStopped = true;
			}

			//MovementRadiusChange();
			//MoveEndCheck();
			//void MovementRadiusChange()
			//{
			//	if(isMove)
			//	{
			//		if(speed > 0f)
			//		{
			//			float disatanceRate = (remainingDistance + stoppingDistance)/ speed;
			//			if(disatanceRate > 1) disatanceRate = 1;
			//			if(disatanceRate < 0.5f) disatanceRate = 0.5f;
			//			navMeshAgent.radius = baseRadius * disatanceRate;

			//			if(disatanceRate < 1f)
			//			{
			//				float speedRate = velocity / speed;
			//				if(speedRate < 0.5f)
			//				{
			//					stoppingDistance = stoppingDistance + Time.deltaTime * (1f-speedRate);
			//				}
			//				else
			//				{
			//					stoppingDistance = stoppingDistance - Time.deltaTime * speedRate;
			//				}
			//				if(stoppingDistance < halfRadius) stoppingDistance = halfRadius;
			//				navMeshAgent.stoppingDistance = stoppingDistance;
			//			}
			//		}
			//	}
			//	else
			//	{
			//		if(stoppingDistance != halfRadius)
			//		{
			//			navMeshAgent.stoppingDistance = halfRadius;
			//			stoppingDistance = halfRadius;
			//		}

			//		if(radius != baseRadius)
			//			navMeshAgent.radius = Mathf.MoveTowards(radius, baseRadius, Time.deltaTime * speed * 0.25f);
			//	}
			//}

			//void MoveEndCheck()
			//{
			//	if(!navMeshAgent.pathPending && isMove)
			//	{
			//		if(stoppingDistance >= remainingDistance)
			//		{

			//			navMeshAgent.stoppingDistance = halfRadius;

			//			aiStateControl.RemoveAIState(FireunitStateTag.MOVE);
			//		}
			//	}
			//}
		}
	}
}
