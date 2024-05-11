using System.Collections.Generic;

using BC.ODCC;

using Sirenix.OdinInspector;

using static BC.GamePlayerManager.StartGameSetting;

namespace BC.GamePlayerManager
{
	public class StartLevelData : DataObject
	{
#if UNITY_EDITOR
		[AssetSelector]
		[ValidateInput("@IsMustNotNull(gameSetting)", "Is Must Not Null")]
		[ValidateInput("@gameSetting == null || IsMustNotNull(UnitSetting)", "UnitSetting Must Not Null")]
		[ValidateInput("@gameSetting == null || IsMustNotNull(MapSetting)", "MapSetting Must Not Null")]
#endif
		public StartGameSetting gameSetting;

		public StartUnitSetting UnitSetting => gameSetting?.unitSetting;
		public StartMapSetting MapSetting => gameSetting?.mapSetting;
		public List<SpawnAnchor> SpawnList => gameSetting.SpawnList;
	}
}
