using BC.ODCC;

using UnityEngine;

namespace BC.Character
{
	public class WeaponModel : ComponentBehaviour
	{
#if UNITY_EDITOR
		public override void BaseReset() { }
		public override void BaseValidate() { }
#endif

		public Transform WeaponPivot;
		public Transform leftHandPivot;
		public Transform rightHandPivot;
		public Transform muzzlePivot;
		public Transform ejectionPort;
		public override void BaseAwake()
		{

		}
		public override void BaseDestroy()
		{

		}
		public override void BaseEnable()
		{

		}
		public override void BaseDisable()
		{

		}
		public override void BaseStart()
		{

		}
		public override void BaseUpdate()
		{

		}
	}
}
