using BC.ODCC;

namespace BC.LowLevelAI
{
	public interface IGetLowLevelAIManager : IOdccObject
	{
		LowLevelAIManager LowLevelAI { get; }
	}

	public class LowLevelAIManager : ObjectBehaviour
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
		}

		public override void BaseAwake()
		{

		}
		public override void BaseDestroy()
		{

		}

	}
}
