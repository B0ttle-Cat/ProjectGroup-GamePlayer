using BC.Base;

namespace BC.GameBaseInterface
{
	public interface IMapModelData
	{
		public ResourcesKey MapNavmeshKey { get; set; }
		public ResourcesKey MapAnchorKey { get; set; }
	}
}
