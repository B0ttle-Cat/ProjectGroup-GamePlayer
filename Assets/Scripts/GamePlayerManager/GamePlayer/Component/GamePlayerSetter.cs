using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class GamePlayerSetter : ComponentBehaviour, IStartSetup
	{
		[ShowInInspector, ReadOnly]
		private bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;

				return isCompleteSetting;
			}
			set { isCompleteSetting=value; }
		}


		public override void BaseAwake()
		{
			base.BaseAwake();
			IsCompleteSetting = false;
		}

		public override void BaseStart()
		{
			base.BaseStart();
			IsCompleteSetting = true;
		}
	}
}
