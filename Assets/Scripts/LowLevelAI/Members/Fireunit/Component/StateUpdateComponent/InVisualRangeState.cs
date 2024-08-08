using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	public class InVisualRangeState : ComponentBehaviour
	{
		private FireunitInteractiveTargetData InteractiveTargetData { get; set; }


		public List<UnitInteractiveInfo> EnemyTargetList = new List<UnitInteractiveInfo>();
		public List<UnitInteractiveInfo> NeutralTargetList = new List<UnitInteractiveInfo>();
		public List<UnitInteractiveInfo> AllianceTargetList = new List<UnitInteractiveInfo>();
		public List<UnitInteractiveInfo> EqualTargetList = new List<UnitInteractiveInfo>();

		public override void BaseAwake()
		{
			base.BaseAwake();

			ThisContainer.NextGetData<FireunitInteractiveTargetData>(interactiveTargetData => InteractiveTargetData = interactiveTargetData);
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			InteractiveTargetData = null;
		}
		public override void BaseEnable()
		{
			base.BaseEnable();
		}
		public override void BaseDisable()
		{
			base.BaseDisable();
		}
		public override void BaseUpdate()
		{
			base.BaseUpdate();
			if(InteractiveTargetData == null) return;


		}
	}
}
