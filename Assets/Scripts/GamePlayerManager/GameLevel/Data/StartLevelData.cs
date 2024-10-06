using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class StartLevelData : DataObject
	{
#if UNITY_EDITOR
		[AssetSelector]
		[ValidateInput("@GamePlaySetting != null", "Is Must Not Null")]
#endif
		public MainPlaySetting GamePlaySetting;

		public FactionSetting FactionSetting => GamePlaySetting?.FactionSetting;
		public UnitSetting UnitSetting => GamePlaySetting?.UnitSetting;
		public MapSetting MapSetting => GamePlaySetting?.MapSetting;
		public TeamSetting TeamSetting => GamePlaySetting?.TeamSetting;
		public CharacterSetting CharacterSetting => GamePlaySetting?.CharacterSetting;
	}
}
