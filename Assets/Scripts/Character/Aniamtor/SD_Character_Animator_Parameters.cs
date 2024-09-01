/// 이 코드는 AnimatorInfo.ParameterEditor.cs 에서 자동생성 됩니다.
using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	public class SD_Character_Animator_Parameters : AnimatorParameterControler
	{
		[ShowInInspector, ReadOnly] public float Move_Speed { get { return GetFloat("Move_Speed"); } set { SetFloat("Move_Speed", value); } }
		[ShowInInspector, ReadOnly] public bool Aiming { get { return GetBool("Attack"); } set { SetBool("Attack", value); } }
		[ShowInInspector, ReadOnly] public float Aim_Right { get { return GetFloat("Aim_Right"); } set { SetFloat("Aim_Right", value); } }
		[ShowInInspector, ReadOnly] public float Aim_Up { get { return GetFloat("Aim_Up"); } set { SetFloat("Aim_Up", value); } }
		[ShowInInspector, ReadOnly] public bool On_FIre { get { return GetBool("On_FIre"); } set { SetTrigger("On_FIre", value); } }
		[ShowInInspector, ReadOnly] public bool On_Reload { get { return GetBool("On_Reload"); } set { SetTrigger("On_Reload", value); } }
		[ShowInInspector, ReadOnly] public int Face_ID { get { return GetInteger("Face_ID"); } set { SetInteger("Face_ID", value); } }

		private bool GetBool(string paramName) { return ThisAnimator != null ? ThisAnimator.GetBool(paramName) : default; }
		private float GetFloat(string paramName) { return ThisAnimator != null ? ThisAnimator.GetFloat(paramName) : default; }
		private int GetInteger(string paramName) { return ThisAnimator != null ? ThisAnimator.GetInteger(paramName) : default; }

		private void SetBool(string paramName, bool value) { if(ThisAnimator != null) ThisAnimator.SetBool(paramName, value); }
		private void SetFloat(string paramName, float value) { if(ThisAnimator != null) ThisAnimator.SetFloat(paramName, value); }
		private void SetInteger(string paramName, int value) { if(ThisAnimator != null) ThisAnimator.SetInteger(paramName, value); }
		private void SetTrigger(string paramName, bool value) { if(ThisAnimator != null) { if(value) ThisAnimator.SetTrigger(paramName); else ThisAnimator.ResetTrigger(paramName); } }

	}
}