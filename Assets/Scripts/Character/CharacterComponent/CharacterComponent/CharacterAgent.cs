using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.Character
{
	public class CharacterAgent : ComponentBehaviour, ICharacterAgent.Agent, ICharacterAgent.TransformPose, IAnimatorStateChangeListener
	//ICharacterAgent<PlayingID, Vector3>,
	//IAgentMoveStart<CharacterAgent>,
	//IAgentMoveStop<CharacterAgent>
	{
		public AnimatorComponent.State CurrentAnimatorState { get; set; }

		public override void BaseEnable()
		{
			base.BaseEnable();
		}

		void ICharacterAgent.TransformPose.OnUpdatePose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.SetPositionAndRotation(position, rotation);
		}
		void ICharacterAgent.TransformPose.OnUpdateLocalPose(Vector3 position, Quaternion rotation)
		{
			ThisTransform.SetLocalPositionAndRotation(position, rotation);
		}

		void IAnimatorStateChangeListener.OnAnimatorStateEnter(AnimatorComponent.State state)
		{

		}

		void IAnimatorStateChangeListener.OnAnimatorStateExit(AnimatorComponent.State state)
		{

		}
	}
}
