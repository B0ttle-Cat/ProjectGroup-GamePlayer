using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class GamePlayerManager : ObjectBehaviour, ILowLevelAIManager, IHighLevelAIManager
	{
		[SerializeField]
		private LowLevelAIManager lowLevelAIManager;
		[SerializeField]
		private HighLevelAIManager highLevelAIManager;
		public LowLevelAIManager LowLevelAIManager { get => lowLevelAIManager; private set => lowLevelAIManager=value; }
		public HighLevelAIManager HighLevelAIManager { get => highLevelAIManager; private set => highLevelAIManager=value; }

		public override void BaseValidate()
		{
			base.BaseValidate();
			LowLevelAIManager = LowLevelAIManager != null ? LowLevelAIManager : FindAnyObjectByType<LowLevelAIManager>();
			HighLevelAIManager = HighLevelAIManager != null ? HighLevelAIManager : FindAnyObjectByType<HighLevelAIManager>();
		}
	}
}
