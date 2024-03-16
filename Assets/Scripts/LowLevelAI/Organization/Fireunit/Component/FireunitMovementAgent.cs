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

	public class FireunitMovementAgent : ComponentBehaviour, IFireunitStateChangeListener, IFireunitMovement
	{
		protected NavMeshAgent navMeshAgent;
		protected NavMeshObstacle navMeshObstacle;
		protected IFireunitStateControl aiStateControl = null;

		protected float baseRadius;
		protected float halfRadius;

		[ShowInInspector, ReadOnly] protected bool isMove = false;
		[ShowInInspector, ReadOnly] protected bool isAttack = false;

		[ShowInInspector, ReadOnly] protected Vector3 inputTarget;

		[ShowInInspector, ReadOnly]
		private NavMeshPathStatus navMeshPathStatus;

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
				aiStateControl = null;
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
			aiStateControl = ThisObject.ThisContainer.GetComponent<IFireunitStateControl>();
		}

		public void InputMoveTarget(Vector3 target)
		{
			if(navMeshAgent is null) return;

			Vector3 random = Random.onUnitSphere;
			random.y = 0;
			target += random * 5f;
			if(NavMesh.SamplePosition(target, out var navMeshHit, 10f, NavMesh.AllAreas))
			{
				navMeshAgent.stoppingDistance = halfRadius;

				// 찾은 위치로 NavMeshAgent를 이동
				inputTarget = navMeshHit.position;
				if(isMove)
				{
					navMeshAgent.SetDestination(inputTarget);
				}
				else
				{
					aiStateControl.AddedAIState(FireunitStateTag.MOVE);
				}
			}
		}

		public override void BaseUpdate()
		{
			base.BaseUpdate();
			if(navMeshAgent is null || !navMeshAgent.isActiveAndEnabled) return;

			navMeshPathStatus = navMeshAgent.pathStatus;



			float speed = navMeshAgent.speed;
			float remainingDistance = navMeshAgent.remainingDistance;
			float velocity = navMeshAgent.velocity.magnitude;
			//Vector3 steeringTarget = navMeshAgent.steeringTarget;
			float stoppingDistance = navMeshAgent.stoppingDistance;
			float radius = navMeshAgent.radius;



			MovementRadiusChange();
			MoveEndCheck();
			void MovementRadiusChange()
			{
				if(isMove)
				{
					if(speed > 0f)
					{
						float disatanceRate = (remainingDistance + stoppingDistance)/ speed;
						if(disatanceRate > 1) disatanceRate = 1;
						if(disatanceRate < 0.5f) disatanceRate = 0.5f;
						navMeshAgent.radius = baseRadius * disatanceRate;

						if(disatanceRate < 1f)
						{
							float speedRate = velocity / speed;
							if(speedRate < 0.5f)
							{
								stoppingDistance = stoppingDistance + Time.deltaTime * (1f-speedRate);
							}
							else
							{
								stoppingDistance = stoppingDistance - Time.deltaTime * speedRate;
							}
							if(stoppingDistance < halfRadius) stoppingDistance = halfRadius;
							navMeshAgent.stoppingDistance = stoppingDistance;
						}
					}
				}
				else
				{
					if(stoppingDistance != halfRadius)
					{
						navMeshAgent.stoppingDistance = halfRadius;
						stoppingDistance = halfRadius;
					}

					if(radius != baseRadius)
						navMeshAgent.radius = Mathf.MoveTowards(radius, baseRadius, Time.deltaTime * speed * 0.25f);
				}
			}

			void MoveEndCheck()
			{
				if(!navMeshAgent.pathPending && isMove)
				{
					if(stoppingDistance >= remainingDistance)
					{

						navMeshAgent.stoppingDistance = halfRadius;

						aiStateControl.RemoveAIState(FireunitStateTag.MOVE);
					}
				}
			}
		}

		public void FireunitStateChangeListener()
		{
			var tags = aiStateControl.CurrentTags;
			isMove = false;
			isAttack = false;
			for(int i = 0 ; i < tags.Count ; i++)
			{
				FireunitStateTag tag = tags[i];
				if(tag == FireunitStateTag.MOVE)
				{
					isMove = true;
				}
				if(tag == FireunitStateTag.ATTACK)
				{
					isAttack = true;
				}
			}


			if(isAttack)
			{
				if(navMeshObstacle != null)
				{
					navMeshObstacle.enabled = true;
				}
				if(navMeshAgent != null)
				{
					navMeshAgent.enabled = false;
				}
			}
			else if(isMove)
			{
				if(navMeshObstacle != null)
				{
					navMeshObstacle.enabled = false;
				}
				if(navMeshAgent != null)
				{
					navMeshAgent.enabled = true;

					navMeshAgent.isStopped = false;
					navMeshAgent.avoidancePriority = 45;

					navMeshAgent.SetDestination(inputTarget);
				}
			}
			else
			{
				if(navMeshObstacle != null)
				{
					navMeshObstacle.enabled = false;
				}
				if(navMeshAgent != null)
				{
					navMeshAgent.enabled = true;

					navMeshAgent.isStopped = true;
					navMeshAgent.avoidancePriority = 50;
				}
			}
		}
	}
}
