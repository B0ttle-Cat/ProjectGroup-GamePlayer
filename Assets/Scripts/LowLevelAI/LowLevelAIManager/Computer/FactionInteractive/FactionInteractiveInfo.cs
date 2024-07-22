using BC.OdccBase;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveValue"/>
	/// </summary>
	public class FactionInteractiveInfo
	{
		public IFactionInteractiveValue Actor { get; private set; }
		public IFactionInteractiveValue Target { get; private set; }
		public FactionInteractiveInfo(IFactionInteractiveValue actor, IFactionInteractiveValue target)
		{
			Actor=actor;
			Target=target;
		}

		public FactionDiplomacyType FactionDiplomacy; // 세력간 관계
	}
}
