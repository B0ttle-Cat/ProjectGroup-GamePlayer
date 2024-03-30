using BC.ODCC;

namespace BC.HighLevelAI
{
	public interface IGetHighLevelAIManager : IOdccObject
	{
		ObjectBehaviour HighLevelAI { get; }
	}
	public class HighLevelAIManager : ObjectBehaviour
	{

	}
}
