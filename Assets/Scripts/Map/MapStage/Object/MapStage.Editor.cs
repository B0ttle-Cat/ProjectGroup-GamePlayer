#if UNITY_EDITOR
using BC.ODCC;

namespace BC.Map
{
	public partial class MapStage : ObjectBehaviour // Editor
	{
		public override void BaseReset()
		{
			base.BaseReset();
		}

		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		private void UpdateMapStageInfo()
		{

		}
	}
}
#endif
