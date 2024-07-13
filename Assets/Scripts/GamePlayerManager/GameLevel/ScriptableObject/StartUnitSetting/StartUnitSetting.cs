using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "StartUnitSetting", menuName = "BC/StartSetting/new StartUnitSetting")]
	public partial class StartUnitSetting : ScriptableObject
	{
		[InfoBox("@DuplicatesMessage", "IsDouble_CharacterDatas", InfoMessageType = InfoMessageType.Error)]
		[TableList]
		public List<StartUnitSettingCharacter> characterDatas;
	}
}