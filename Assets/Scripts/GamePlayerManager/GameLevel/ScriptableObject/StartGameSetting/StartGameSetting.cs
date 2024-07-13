using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "StartGameSetting", menuName = "BC/StartSetting/new StartGameSetting/")]
	public partial class StartGameSetting : ScriptableObject
	{
		[AssetSelector, ValidateInput("@mapSetting != null", "Is Must Not Null")]
		[PropertyOrder(-100)]
		public StartMapSetting mapSetting;
		[AssetSelector, ValidateInput("@unitSetting != null", "Is Must Not Null")]
		[PropertyOrder(-100)]
		public StartUnitSetting unitSetting;
		[AssetSelector, ValidateInput("@unitSetting != null", "Is Must Not Null")]
		[PropertyOrder(-100)]
		public StartFactionSetting factionSetting;

		[TabGroup("Tap", nameof(SpawnList)), TableList, PropertyOrder(0)]
		public List<SpawnAnchor> SpawnList;

		[TabGroup("Tap", nameof(TeamGizmosInfo)), TableList, PropertyOrder(100)]
		public List<GizmosInfo> TeamGizmosInfo;


#if UNITY_EDITOR
		public void OnValidate()
		{
			mapSetting?.ConnectStartGameSetting(this);
			unitSetting?.ConnectStartGameSetting(this);
			factionSetting?.ConnectStartGameSetting(this);
		}
#endif
	}
	public interface IConnectStartGameSetting_Editor
	{
#if UNITY_EDITOR
		public StartGameSetting startGameSetting { get; set; }
		void ConnectStartGameSetting(StartGameSetting startGameSetting);
#endif
	}
}
