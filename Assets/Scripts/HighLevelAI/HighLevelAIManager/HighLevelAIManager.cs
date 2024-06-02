using BC.ODCC;

namespace BC.HighLevelAI
{
	public interface IGetHighLevelAIManager : IOdccObject
	{
		HighLevelAIManager HighLevelAI { get; }
	}
	public class HighLevelAIManager : ObjectBehaviour
	{

	}
}
