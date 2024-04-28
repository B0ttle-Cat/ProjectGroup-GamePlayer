/// 이 코드는 AnimatorInfo.ParameterEditor.cs 에서 자동생성 됩니다.
using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	public class SD_Character_Animator_Parameters : AnimatorParameterControler
	{
		[ShowInInspector, ReadOnly] public float Move_Speed { get { return GetFloat("Move_Speed"); } set { SetFloat("Move_Speed", value); } }
		[ShowInInspector, ReadOnly] public bool Aimming { get { return GetBool("Aimming"); } set { SetBool("Aimming", value); } }
		[ShowInInspector, ReadOnly] public float Aim_Right { get { return GetFloat("Aim_Right"); } set { SetFloat("Aim_Right", value); } }
		[ShowInInspector, ReadOnly] public float Aim_Up { get { return GetFloat("Aim_Up"); } set { SetFloat("Aim_Up", value); } }
		[ShowInInspector, ReadOnly] public bool On_FIre { get { return GetBool("On_FIre"); } set { SetTrigger("On_FIre", value);} }
		[ShowInInspector, ReadOnly] public bool On_Reload { get { return GetBool("On_Reload"); } set { SetTrigger("On_Reload", value);} }
		[ShowInInspector, ReadOnly] public int Face_ID { get { return GetInteger("Face_ID"); } set { SetInteger("Face_ID", value); } }

		private bool GetBool(string paramName) { return animator != null ? animator.GetBool(paramName) : default; }
		private float GetFloat(string paramName) { return animator != null ? animator.GetFloat(paramName) : default; }
		private int GetInteger(string paramName) { return animator != null ? animator.GetInteger(paramName) : default; }

		private void SetBool(string paramName, bool value) { if(animator != null) animator.SetBool(paramName, value); }
		private void SetFloat(string paramName, float value) { if(animator != null) animator.SetFloat(paramName, value); }
		private void SetInteger(string paramName, int value) { if(animator != null) animator.SetInteger(paramName, value); }
		private void SetTrigger(string paramName, bool value) { if(animator != null) { if(value) animator.SetTrigger(paramName); else animator.ResetTrigger(paramName); } }

	}
}