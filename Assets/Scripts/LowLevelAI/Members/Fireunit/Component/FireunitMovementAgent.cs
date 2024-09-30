using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

namespace BC.LowLevelAI
{
	public class FireunitMovementAgent : ComponentBehaviour, IUnitIMovementAgent
	{
		private NavMeshAgent navMeshAgent;
		protected NavMeshObstacle navMeshObstacle;
		[SerializeField, ReadOnly]
		protected Vector3 inputVectorTarget;
		[SerializeField, ReadOnly]
		protected IMapPathNode inputNodeTarget;
		[SerializeField, ReadOnly]
		protected Vector3 inputFormationPosition;

		protected float baseRadius;
		protected float halfRadius;
		private const float nextNodeDistance = 5f;

		private NavMeshPathStatus navMeshPathStatus;
		private NavMeshPath navMeshPath;
		public bool IsMove;

		public NavMeshAgent NavMeshAgent { get => navMeshAgent; private set => navMeshAgent=value; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			InitAgent();
		}
		public virtual void InitAgent()
		{
			NavMeshAgent = ThisObject.GetComponentInChildren<NavMeshAgent>();
			if(NavMeshAgent == null)
			{
				NavMeshAgent = null;
				return;
			}

			navMeshObstacle = ThisObject.GetComponentInChildren<NavMeshObstacle>();
			if(navMeshObstacle == null)
			{
				navMeshObstacle = null;
			}

			baseRadius = NavMeshAgent.radius;
			halfRadius = baseRadius * 0.5f;

			NavMeshAgent.stoppingDistance = halfRadius;

			navMeshPath = new NavMeshPath();
		}

		public void InputMoveStop(Vector3? stopWarp = null)
		{
			if(NavMeshAgent is null || navMeshPath is null) return;

			NavMeshAgent.ResetPath();
			if(stopWarp.HasValue)
			{
				NavMeshAgent.Warp(stopWarp.Value);
			}
			NavMeshAgent.isStopped = true;
			IsMove = false;
		}
		public void InputMoveTarget(Vector3 target)
		{
			InputMoveTarget(target, false);
		}
		public void InputMoveTarget(Vector3 target, bool isWarp)
		{
			if(NavMeshAgent is null || navMeshPath is null) return;
			if(NavMesh.SamplePosition(target, out var navMeshHit, 10f, NavMesh.AllAreas))
			{
				target = navMeshHit.position;
			}

			inputVectorTarget = target;
			if(isWarp)
			{
				NavMeshAgent.ResetPath();
				NavMeshAgent.Warp(inputVectorTarget);
				NavMeshAgent.isStopped = true;
				IsMove = false;
				return;
			}

			NavMeshAgent.isStopped = false;
			IsMove = true;

			if(NavMeshAgent.CalculatePath(inputVectorTarget, navMeshPath))
			{
				if(NavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
				{
					NavMeshAgent.SetDestination(inputVectorTarget);
					navMeshPath = NavMeshAgent.path;
				}
				else
				{
					NavMeshAgent.SetPath(navMeshPath);
				}
			}
			else
			{
				NavMeshAgent.SetDestination(inputVectorTarget);
				navMeshPath = NavMeshAgent.path;
			}
		}
		internal void InputMoveTarget(IMapPathNode target, Vector3 formationPosition)
		{
			if(NavMeshAgent is null) return;

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
			Vector3 currPosition = NavMeshAgent.nextPosition;

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
			if(NavMeshAgent is null) return;

			navMeshPath = target;
			inputVectorTarget = target.corners[^1];
			NavMeshAgent.SetPath(target);
			NavMeshAgent.isStopped = false;
			IsMove = true;
		}
		public override void BaseUpdate()
		{
			base.BaseUpdate();

			if(NavMeshAgent is null || navMeshPath is null || !NavMeshAgent.isActiveAndEnabled)
			{
				return;
			}

			Vector3 currentPos = NavMeshAgent.nextPosition;

			MoveToPosition();
			MoveEndCheck();

			void MoveToPosition()
			{
				if(!IsMove) return;

				navMeshPathStatus = NavMeshAgent.pathStatus;
				bool hasPath = NavMeshAgent.hasPath;
				if(!hasPath)
				{
					IsMove = false;
					return;
				}
				bool isPathStale = NavMeshAgent.isPathStale;
				if(isPathStale)
				{
					IsMove = false;
					InputMoveTarget(inputVectorTarget);
					return;
				}
				bool pathPending = NavMeshAgent.pathPending;

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


					float velocity = NavMeshAgent.velocity.magnitude;
					velocity += NavMeshAgent.acceleration * Time.deltaTime;

					float speed = NavMeshAgent.speed;
					if(velocity > speed)
					{
						velocity = speed;
					}

					NavMeshAgent.Move(diraction.normalized * velocity);
				}
				else if(navMeshPathStatus == NavMeshPathStatus.PathPartial || navMeshPathStatus == NavMeshPathStatus.PathInvalid)
				{
					NavMeshAgent.SetDestination(inputVectorTarget);
					navMeshPath = NavMeshAgent.path;
				}
			}
			void MoveEndCheck()
			{
				if(!IsMove) return;

				float remainingDistance = NavMeshAgent.remainingDistance;
				float stoppingDistance = NavMeshAgent.stoppingDistance;
				float radius = NavMeshAgent.radius;

				if(inputNodeTarget == null || inputNodeTarget.ThisPoint == null)
				{
					if(remainingDistance <= stoppingDistance)
					{
						IsMove = false;
						NavMeshAgent.isStopped = true;
					}
				}
				else
				{
					if(inputNodeTarget.NextNode == null)
					{
						if(remainingDistance <= stoppingDistance)
						{
							IsMove = false;
							NavMeshAgent.isStopped = true;
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
			Vector3 currentPos = NavMeshAgent.nextPosition;


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
