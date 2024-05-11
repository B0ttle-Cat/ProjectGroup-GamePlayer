using BC.ODCC;

namespace BC.LowLevelAI
{
	public partial class MapStage : ObjectBehaviour
	{
		private MapStageData mapStageData;
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			mapStageData = null;
		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<MapStageData>(out mapStageData))
			{
				mapStageData = ThisContainer.AddData<MapStageData>();
			}
		}
	}
}
