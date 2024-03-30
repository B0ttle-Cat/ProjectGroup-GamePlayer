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
	}
}
