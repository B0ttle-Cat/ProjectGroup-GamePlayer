using BC.ODCC;

namespace BC.OdccBase
{
	public interface IMapCreatorComponent : IOdccComponent
	{
		public IMapSetting MapSetting { get; set; }
	}
}
