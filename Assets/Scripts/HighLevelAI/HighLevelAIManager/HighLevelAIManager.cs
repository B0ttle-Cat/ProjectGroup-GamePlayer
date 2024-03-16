using BC.ODCC;

namespace BC.HighLevelAI
{
	public interface IHighLevelAIManager : IOdccObject
	{
		HighLevelAIManager HighLevelAIManager { get; }
	}
	public class HighLevelAIManager : ObjectBehaviour
	{

	}
}
