using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class GamePlayerManager : ObjectBehaviour, IGetLowLevelAIManager, IGetHighLevelAIManager
	{
		[SerializeField]
		private LowLevelAIManager lowLevelAIManager;
		[SerializeField]
		private HighLevelAIManager highLevelAIManager;
		public LowLevelAIManager LowLevelAI { get => lowLevelAIManager; }
		public HighLevelAIManager HighLevelAI { get => highLevelAIManager; }

		public override void BaseValidate()
		{
			base.BaseValidate();

			lowLevelAIManager = lowLevelAIManager != null ? lowLevelAIManager : FindAnyObjectByType<LowLevelAIManager>();
			highLevelAIManager = highLevelAIManager != null ? highLevelAIManager : FindAnyObjectByType<HighLevelAIManager>();
		}
		public override void BaseAwake()
		{
			base.BaseAwake();

			lowLevelAIManager = lowLevelAIManager != null ? lowLevelAIManager : FindAnyObjectByType<LowLevelAIManager>();
			highLevelAIManager = highLevelAIManager != null ? highLevelAIManager : FindAnyObjectByType<HighLevelAIManager>();
		}
		public override void BaseDestroy()
		{
			lowLevelAIManager  = null;
			highLevelAIManager = null;
		}

		public override void BaseEnable()
		{
			if(ThisContainer.TryGetComponent<GameLevelSetter>(out var levelSetter))
			{
				if(levelSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = true;
				}
			}

			if(ThisContainer.TryGetComponent<GamePlayerSetter>(out var playerSetter))
			{
				if(playerSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = true;
				}
			}

			if(ThisContainer.TryGetComponent<GameUnitSetter>(out var unitSetter))
			{
				if(unitSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = true;
				}
			}
		}

		public override void BaseDisable()
		{
			if(ThisContainer.TryGetComponent<GameLevelSetter>(out var levelSetter))
			{
				if(levelSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = false;
				}
			}

			if(ThisContainer.TryGetComponent<GamePlayerSetter>(out var playerSetter))
			{
				if(playerSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = false;
				}
			}

			if(ThisContainer.TryGetComponent<GameUnitSetter>(out var unitSetter))
			{
				if(unitSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = false;
				}
			}
		}
	}
}
