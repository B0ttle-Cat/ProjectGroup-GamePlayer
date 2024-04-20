using BC.ODCC;
using BC.OdccBase;

using UnityEngine;
using UnityEngine.AI;

namespace BC.Character
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class CharacterAgent : ComponentBehaviour, IAgentToCharacter
	//ICharacterAgent<PlayingID, Vector3>,
	//IAgentMoveStart<CharacterAgent>,
	//IAgentMoveStop<CharacterAgent>
	{
		private CharacterAnimator characterAnimation;

		public override void BaseEnable()
		{
			base.BaseEnable();
			characterAnimation = null;
		}


		void IAgentToCharacter.OnUpdatePose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.position = position;
			ThisTransform.rotation = rotation;
		}

		void IAgentToCharacter.OnUpdateMovment(Vector3 velocity)
		{
			if(characterAnimation == null && !ThisContainer.TryGetComponent<CharacterAnimator>(out characterAnimation))
			{
				return;
			}
			//characterAnimation.
		}
	}
}
