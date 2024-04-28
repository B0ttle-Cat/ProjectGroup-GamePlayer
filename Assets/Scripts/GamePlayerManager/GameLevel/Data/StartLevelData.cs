using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class StartLevelData : DataObject
	{
#if UNITY_EDITOR
		[AssetSelector, ValidateInput("@IsMustNotNull(unitSetting, mapSetting)", "Is Must Not Null")]
#endif
		public StartUnitSetting unitSetting;
#if UNITY_EDITOR
		[AssetSelector]
#endif
		public StartMapSetting mapSetting;
	}
}
