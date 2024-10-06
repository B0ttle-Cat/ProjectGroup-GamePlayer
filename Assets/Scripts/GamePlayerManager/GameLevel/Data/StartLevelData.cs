using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class StartLevelData : DataObject
	{
#if UNITY_EDITOR
		[AssetSelector]
		[ValidateInput("@GameSetting != null", "Is Must Not Null")]
#endif
		public MainPlaySetting GameSetting;

		public FactionSetting FactionSetting => GameSetting?.FactionSetting;
		public UnitSetting UnitSetting => GameSetting?.UnitSetting;
		public MapSetting MapSetting => GameSetting?.MapSetting;
		public TeamSetting TeamSetting => GameSetting?.TeamSetting;
		public CharacterSetting CharacterSetting => GameSetting?.CharacterSetting;
	}
}
