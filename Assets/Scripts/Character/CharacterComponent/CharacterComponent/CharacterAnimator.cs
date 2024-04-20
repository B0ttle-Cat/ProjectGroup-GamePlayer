using System;

using BC.ODCC;

using UnityEngine;

namespace BC.Character
{
	[RequireComponent(typeof(Animator))]
	public class CharacterAnimator : ComponentBehaviour
	//IAgentMoveStart<CharacterAgent>,
	//IAgentMoveStop<CharacterAgent>,
	//IWeaponFire,
	//IWeaponReload
	{
		public Animator animator;

		public const string KEY_F_MOVE_SPEED = "Move_Speed";
		public const string KEY_B_AIMMING = "Aimming";
		public const string KEY_F_AIM_FORWARD = "Aim_Forward";
		public const string KEY_F_AIM_BACK = "Aim_Right";
		public const string KEY_F_AIM_RIGHT = "Aim_Back";
		public const string KEY_F_AIM_LEFT = "Aim_Left";
		public const string KEY_F_AIM_UP = "Aim_Up";
		public const string KEY_F_AIM_DOWN = "Aim_Down";
		public const string KEY_T_ON_FIRE = "On_FIre";
		public const string KEY_T_ON_RELOAD = "On_Reload";
		public const string KEY_I_FACE_ID = "Face_ID";

		private Action agentMoveUpdate;

		public override void BaseAwake()
		{
			animator = GetComponent<Animator>();
			if(animator == null) animator = null;
			agentMoveUpdate = null;
		}
		public override void BaseDestroy()
		{
			animator = null;
			agentMoveUpdate = null;
		}

		public override void BaseEnable()
		{
			if(animator is null) return;
			animator.enabled = true;
		}

		public override void BaseUpdate()
		{
			agentMoveUpdate?.Invoke();
		}

		//void IWeaponFire.DoFire()
		//{
		//	if(animator is null) return;
		//	animator.enabled = false;
		//}
		//
		//void IWeaponReload.DoReload()
		//{
		//
		//}
		//
		//void IAgentMoveStart<CharacterAgent>.DoAgentMoveStart(CharacterAgent agent, Vector3 target)
		//{
		//	if(animator is null || agent is null) return;
		//
		//	agentMoveUpdate = () =>
		//	{
		//		if(animator is null || agent is null)
		//		{
		//			agentMoveUpdate = null;
		//			return;
		//		}
		//		//float realMoveSpeed = agent?.NavAgent.velocity.magnitude ?? 0f;
		//		//animator.SetFloat(KEY_F_MOVE_SPEED, realMoveSpeed);
		//	};
		//}
		//
		//void IAgentMoveStop<CharacterAgent>.DoAgentMoveStop(CharacterAgent agent)
		//{
		//	if(animator is null) return;
		//
		//	agentMoveUpdate = null;
		//	animator.SetFloat(KEY_F_MOVE_SPEED, 0f);
		//}
	}
}
