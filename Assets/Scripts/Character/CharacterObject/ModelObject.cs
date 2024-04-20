using BC.ODCC;

namespace BC.Character
{
	public abstract class ModelObject : ObjectBehaviour
	{
		public abstract bool IsReady { get; protected set; }
	}
}
