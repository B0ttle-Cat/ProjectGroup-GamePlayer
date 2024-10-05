using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class SubPlaySetting : ScriptableObject
	{
		[ReadOnly,PropertyOrder(float.MinValue)]
		public MainPlaySetting mainPlaySetting;
	}
}
