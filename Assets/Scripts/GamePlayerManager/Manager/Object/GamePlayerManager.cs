using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class GamePlayerManager : ObjectBehaviour, IGetLowLevelAIManager, IGetHighLevelAIManager
	{
		[SerializeField]
		private LowLevelAIManager lowLevelAIManager;
		[SerializeField]
		private HighLevelAIManager highLevelAIManager;
		public ObjectBehaviour LowLevelAI { get => lowLevelAIManager; }
		public ObjectBehaviour HighLevelAI { get => highLevelAIManager; }

		public override void BaseValidate()
		{
			base.BaseValidate();

			lowLevelAIManager = lowLevelAIManager != null ? lowLevelAIManager : FindAnyObjectByType<LowLevelAIManager>();
			highLevelAIManager = highLevelAIManager != null ? highLevelAIManager : FindAnyObjectByType<HighLevelAIManager>();
		}


		public override void BaseEnable()
		{
			if(ThisContainer.TryGetComponent<GameLevelSetter>(out var levelSetter))
			{
				if(levelSetter is IStartSetup iSetter)
				{
					iSetter.OnStartSetting();
				}
			}

			if(ThisContainer.TryGetComponent<GamePlayerSetter>(out var playerSetter))
			{
				if(playerSetter is IStartSetup iSetter)
				{
					iSetter.OnStartSetting();
				}
			}
		}

		public override void BaseDisable()
		{
			if(ThisContainer.TryGetComponent<GameLevelSetter>(out var levelSetter))
			{
				if(levelSetter is IStartSetup iSetter)
				{
					iSetter.OnStopSetting();
				}
			}

			if(ThisContainer.TryGetComponent<GamePlayerSetter>(out var playerSetter))
			{
				if(playerSetter is IStartSetup iSetter)
				{
					iSetter.OnStopSetting();
				}
			}
		}
	}
}
