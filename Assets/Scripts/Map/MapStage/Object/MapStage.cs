using BC.ODCC;

namespace BC.Map
{
	public partial class MapStage : ObjectBehaviour
	{
		private MapStageData mapStageData;
		protected override void Disposing()
		{
			base.Disposing();
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
