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
		[ValidateInput("@gameSetting != null", "Is Must Not Null")]
		[ValidateInput("@UnitSetting != null", "UnitSetting Must Not Null")]
		[ValidateInput("@MapSetting != null", "MapSetting Must Not Null")]
#endif
		public StartGameSetting gameSetting;

		public StartUnitSetting UnitSetting => gameSetting?.unitSetting;
		public StartMapSetting MapSetting => gameSetting?.mapSetting;
		public StartFactionSetting FactionSetting => gameSetting?.factionSetting;
		public List<SpawnAnchor> SpawnList => gameSetting.SpawnList;
	}
}
