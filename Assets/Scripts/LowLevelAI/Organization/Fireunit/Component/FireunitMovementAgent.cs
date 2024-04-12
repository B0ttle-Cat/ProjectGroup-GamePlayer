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
		protected Vector3 inputVectorTarget;
		[ShowInInspector, ReadOnly]
		protected MapPathNode inputNodeTarget;
		[ShowInInspector, ReadOnly]
		protected Vector3 inputFormationPosition;

		protected float baseRadius;
		protected float halfRadius;
		private const float nextNodeDistance = 5f;

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

			inputVectorTarget = target;
			if(isWarp)
			{
				navMeshAgent.ResetPath();
				navMeshAgent.Warp(inputVectorTarget);
				navMeshAgent.isStopped = true;
				IsMove = false;
				return;
			}

			navMeshAgent.isStopped = false;
			IsMove = true;

			if(navMeshAgent.CalculatePath(inputVectorTarget, navMeshPath))
			{
				if(navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
				{
					navMeshAgent.SetDestination(inputVectorTarget);
					navMeshPath = navMeshAgent.path;
				}
				else
				{
					navMeshAgent.SetPath(navMeshPath);
				}
			}
			else
			{
				navMeshAgent.SetDestination(inputVectorTarget);
				navMeshPath = navMeshAgent.path;
			}
		}
		internal void InputMoveTarget(MapPathNode target, Vector3 formationPosition)
		{
			if(navMeshAgent is null) return;

			inputNodeTarget = target;
			inputFormationPosition = formationPosition;

			if(inputNodeTarget == null)
			{
				InputMoveStop();
				return;
			}
			var thisPoint = inputNodeTarget.ThisPoint;
			if(thisPoint == null)
			{
				InputMoveStop();
				return;
			}

			Vector3 thisPosition  = thisPoint.ThisPosition();

			// 이 노드가 마지막 노드임.
			if(inputNodeTarget.NextNode == null)
			{
				if(NavMesh.Raycast(thisPosition, thisPosition += inputFormationPosition, out var hit, NavMesh.AllAreas))
				{
					thisPosition = hit.position;
				}
				else
				{
					thisPosition += inputFormationPosition;
				}
				InputMoveTarget(thisPosition);
				return;
			}
			var nextPoint = inputNodeTarget.NextNode.ThisPoint;
			if(nextPoint == null)
			{
				InputMoveTarget(thisPosition);
				return;
			}

			Vector3 nextPosition = nextPoint.ThisPosition();
			Vector3 currPosition = navMeshAgent.nextPosition;

			NavMeshPath thisToNextPath = new NavMeshPath();
			NavMeshPath currToNextPath = new NavMeshPath();
			float thisToNext = 0;
			float currToNext = 0;
			if(NavMesh.CalculatePath(thisPosition, nextPosition, NavMesh.AllAreas, thisToNextPath))
			{
				thisToNext = TotalDistanceThisPath(thisToNextPath);
			}
			if(NavMesh.CalculatePath(currPosition, nextPosition, NavMesh.AllAreas, currToNextPath))
			{
				currToNext = TotalDistanceThisPath(currToNextPath);
			}
			if(thisToNext < currToNext)
			{
				// 보통 이게 일반적임.
				InputMoveTarget(thisPosition);
			}
			else
			{
				// 이미 This 를 넘어가서 Next 에 더 가까운 상태
				InputMoveTarget(currToNextPath);
			}
		}
		internal void InputMoveTarget(NavMeshPath target)
		{
			if(navMeshAgent is null) return;

			navMeshPath = target;
			inputVectorTarget = target.corners[^1];
			navMeshAgent.SetPath(target);
			navMeshAgent.isStopped = false;
			IsMove = true;
		}
		public override void BaseUpdate()
		{
			base.BaseUpdate();

			if(navMeshAgent is null || navMeshPath is null || !navMeshAgent.isActiveAndEnabled)
			{
				return;
			}

			Vector3 currentPos = navMeshAgent.nextPosition;

			MoveToPosition();
			MoveEndCheck();

			void MoveToPosition()
			{
				if(!IsMove) return;

				navMeshPathStatus = navMeshAgent.pathStatus;
				bool hasPath = navMeshAgent.hasPath;
				if(!hasPath)
				{
					IsMove = false;
					return;
				}
				bool isPathStale = navMeshAgent.isPathStale;
				if(isPathStale)
				{
					IsMove = false;
					InputMoveTarget(inputVectorTarget);
					return;
				}
				bool pathPending = navMeshAgent.pathPending;

				if(navMeshPathStatus == NavMeshPathStatus.PathComplete)
				{
					// 경로를 올바르게 찾음 
				}
				else if(pathPending)
				{
					Vector3 diraction = Vector3.zero;

					var corners = navMeshPath.corners;
					if(corners.Length < 2)
					{
						diraction = currentPos - inputVectorTarget;
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
					navMeshAgent.SetDestination(inputVectorTarget);
					navMeshPath = navMeshAgent.path;
				}
			}
			void MoveEndCheck()
			{
				if(!IsMove) return;

				float remainingDistance = navMeshAgent.remainingDistance;
				float stoppingDistance = navMeshAgent.stoppingDistance;
				float radius = navMeshAgent.radius;

				if(inputNodeTarget == null || inputNodeTarget.ThisPoint == null)
				{
					if(remainingDistance <= stoppingDistance)
					{
						IsMove = false;
						navMeshAgent.isStopped = true;
					}
				}
				else
				{
					if(inputNodeTarget.NextNode == null)
					{
						if(remainingDistance <= stoppingDistance)
						{
							IsMove = false;
							navMeshAgent.isStopped = true;
						}
					}
					else
					{
						//if(remainingDistance < nextNodeDistance)
						{
							var nextNode = inputNodeTarget.NextNode;
							if(!NavMesh.Raycast(currentPos, nextNode.ThisPoint.ThisPosition(), out var hit, NavMesh.AllAreas))
							{
								InputMoveTarget(nextNode, inputFormationPosition);
							}
							else if(remainingDistance <= nextNodeDistance)
							{
								InputMoveTarget(nextNode, inputFormationPosition);
							}
						}
					}
				}

			}
		}

		private float TotalDistanceThisPath(NavMeshPath navMeshPath)
		{
			float distance = 0f;
			Vector3[] corners = navMeshPath.corners;
			for(int i = 0 ; i < corners.Length - 1 ; i++)
			{
				distance += Vector3.Distance(corners[i], corners[i + 1]);
			}
			return distance;
		}

#if UNITY_EDITOR
		[Header("OnDrawGizmos")]
		public bool isOnDrawGizmos;
		private void OnDrawGizmos()
		{
			if(!isOnDrawGizmos) return;
			Vector3 currentPos = navMeshAgent.nextPosition;


			if(inputNodeTarget != null && inputNodeTarget.ThisPoint != null)
			{
				Vector3 position = inputNodeTarget.ThisPoint.ThisPosition();

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(currentPos + Vector3.up * 4f, position + Vector3.up * 4f);
				inputNodeTarget.OnDrawGizmos(Vector3.up * 4f);
			}

		}
#endif
	}
}
