using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IGetLowLevelAIManager : IOdccObject
	{
		LowLevelAIManager LowLevelAI { get; }
		MapStage MapStage { get { return LowLevelAI.MapStage; } }
	}

	public class LowLevelAIManager : ObjectBehaviour
	{
		public MapStage MapStage { get; private set; }
		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		public override void BaseAwake()
		{
			MapStage = ThisContainer.GetChildObject<MapStage>();
		}
		public override void BaseDestroy()
		{

		}
	}
}
