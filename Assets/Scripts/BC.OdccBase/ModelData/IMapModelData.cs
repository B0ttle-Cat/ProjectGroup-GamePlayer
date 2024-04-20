using BC.Base;

namespace BC.OdccBase
{
	public interface IMapModelData
	{
		public ResourcesKey MapNavmeshKey { get; set; }
		public ResourcesKey MapAnchorKey { get; set; }
	}
}
