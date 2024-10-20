using BC.ODCC;

namespace BC.OdccBase
{
	public interface IGamePlayManager : IOdccObject
	{
		public ILowLevelAIManager LowLevelAI { get; }
		public IHighLevelAIManager HighLevelAI { get; }

		public IMapCreatorComponent MapCreatorComponent { get; }
		public ICharacterCreatorComponent CharacterCreatorComponent { get; }
		public IFactionCreatorComponent FactionCreatorComponent { get; }
		public ITeamCreatorComponent TeamCreatorComponent { get; }
		public IUnitCreatorComponent UnitCreatorComponent { get; }
	}
}
