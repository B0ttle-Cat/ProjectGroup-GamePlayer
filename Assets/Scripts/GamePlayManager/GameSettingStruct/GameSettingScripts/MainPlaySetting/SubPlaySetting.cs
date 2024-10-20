using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class SubPlaySetting : ScriptableObject
	{
		[ReadOnly,PropertyOrder(float.MinValue)]
		public MainPlaySetting mainPlaySetting;
	}
}
