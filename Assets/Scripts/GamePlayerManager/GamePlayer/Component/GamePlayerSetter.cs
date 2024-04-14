using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class GamePlayerSetter : ComponentBehaviour, IStartSetup
	{
		[SerializeField,ReadOnly]
		private bool isCompleteSetting;

		public bool IsCompleteSetting { get => isCompleteSetting; set => isCompleteSetting=value; }
		public override void BaseEnable()
		{
			IsCompleteSetting = false;
		}
		public override void BaseUpdate()
		{
			OnUpdateSetting();
		}
		public void OnStartSetting()
		{
			enabled = true;
		}

		public void OnStopSetting()
		{
			enabled = false;
		}

		public void OnUpdateSetting()
		{
			if(IsCompleteSetting)
			{
				OnStopSetting();
				return;
			}


			IsCompleteSetting = true;
		}
	}
}
