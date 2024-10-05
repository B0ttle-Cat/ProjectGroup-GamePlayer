using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class StartLevelData : DataObject
	{
#if UNITY_EDITOR
		[AssetSelector]
		[ValidateInput("@gameSetting != null", "Is Must Not Null")]
		[ValidateInput("@UnitSetting != null", "UnitSetting Must Not Null")]
		[ValidateInput("@MapSetting != null", "MapSetting Must Not Null")]
#endif
		public MainPlaySetting gameSetting;

		public UnitSetting UnitSetting => gameSetting?.UnitSetting;
		public MapSetting MapSetting => gameSetting?.MapSetting;
		public FactionSetting FactionSetting => gameSetting?.FactionSetting;
		public TeamSetting SpawnSetting => gameSetting?.TeamSetting;
	}
}
