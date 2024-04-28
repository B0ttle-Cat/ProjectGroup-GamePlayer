using BC.ODCC;

namespace BC.OdccBase
{
	public interface IGetIAgentToCharacter : IOdccObject
	{
		public ICharacterAgent ToCharacter { get; set; }
	}
	public interface ICharacterToAgent : IOdccComponent
	{
		public ICharacterAgent ToAgent { get { return ThisContainer.GetObject<IGetIAgentToCharacter>()?.ToCharacter; } }
		public IFireunitData UnitData { get { return ThisContainer.GetData<IFireunitData>(); } }

	}
}
