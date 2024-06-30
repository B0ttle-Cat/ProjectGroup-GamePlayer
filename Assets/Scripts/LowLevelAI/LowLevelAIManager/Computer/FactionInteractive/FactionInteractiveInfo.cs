using BC.OdccBase;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="IFactionInteractiveValue"/>
	/// </summary>
	public class FactionInteractiveInfo
	{
		public IFactionInteractiveActor Actor { get; private set; }
		public IFactionInteractiveTarget Target { get; private set; }
		public FactionInteractiveInfo(IFactionInteractiveActor actor, IFactionInteractiveTarget target)
		{
			Actor=actor;
			Target=target;
		}

		public FactionDiplomacyType FactionDiplomacy; // 세력간 관계
	}
}
