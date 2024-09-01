using BC.ODCC;

using UnityEngine;

namespace BC.Character
{

	[RequireComponent(typeof(Animator))]
	public class WeaponAnimation : ComponentBehaviour
	{
		protected WeaponModel model;

		[Header("IAnimation")]
		public Animator animator;

		public override void BaseAwake()
		{
			model = ThisContainer.GetComponent<WeaponModel>();
		}
		public override void BaseEnable()
		{
			if(animator == null) animator = null;
		}
	}
}
