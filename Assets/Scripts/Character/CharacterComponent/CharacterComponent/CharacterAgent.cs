using BC.Base;
using BC.ODCC;

using UnityEngine;
using UnityEngine.AI;

namespace BC.Character
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class CharacterAgent : ComponentBehaviour,
		INavAgent<PlayingID, Vector3>,
		IAgentMoveStart<CharacterAgent>,
		IAgentMoveStop<CharacterAgent>
	{
		public NavMeshAgent NavAgent { get; private set; }
		public PlayingID NavID { get; private set; }

		public override void BaseAwake()
		{
			NavAgent = GetComponent<NavMeshAgent>();
			if(NavAgent == null) NavAgent = null;
		}

		public override void BaseStart()
		{
			NavID = ThisObject.ThisContainer.GetData<PlayingID>();
		}


		void INavAgent<PlayingID, Vector3>.OnMoveStart(INavTarget<PlayingID, Vector3> navTarget)
		{
			ThisObject.ThisContainer.CallActionAllComponent<IAgentMoveStart<CharacterAgent>>((i) => i.DoAgentMoveStart(this, navTarget.NavTargetValue));
		}

		void INavAgent<PlayingID, Vector3>.OnMoveStop()
		{
			ThisObject.ThisContainer.CallActionAllComponent<IAgentMoveStop<CharacterAgent>>((i) => i.DoAgentMoveStop(this));
		}
		void IAgentMoveStart<CharacterAgent>.DoAgentMoveStart(CharacterAgent agent, Vector3 target)
		{
			if(NavAgent is null || !NavAgent.SetDestination(target))
			{
				ThisObject.ThisContainer.CallActionAllComponent<IAgentMoveStop<CharacterAgent>>((i) => i.DoAgentMoveStop(this));
			}
		}
		void IAgentMoveStop<CharacterAgent>.DoAgentMoveStop(CharacterAgent agent)
		{
			if(NavAgent is null)
			{
				NavAgent.isStopped = true;
			}
		}
	}
}
