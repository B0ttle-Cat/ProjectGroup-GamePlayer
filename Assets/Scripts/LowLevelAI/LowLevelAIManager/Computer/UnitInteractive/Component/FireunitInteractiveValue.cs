using System;

using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public partial class FireunitInteractiveValue : ComponentBehaviour, IUnitInteractiveValue
	{
		private Func<UnitInteractiveInfo, bool>[] tacticalStateChecker;
		private IUnitTacticalCombatStateChanger iUnitStateChanger;
		private IUnitTacticalCombatStateUpdate iUnitStateUpdate;

		// Data
		public IFireunitData UnitData { get; private set; }
		public IFindCollectedMembers FindMembers { get; set; }
		public IUnitTypeValue TypeValueData { get; private set; }
		public IUnitPlayValue PlayValueData { get; private set; }
		public IUnitPoseValue PoseValueData { get; private set; }
		public IUnitStateValue StateValueData { get; private set; }
		public IUnitInteractiveInterface InteractiveInterface { get; private set; }
		public ITakeDamage TakeDamage { get; private set; }
		public ITakeRecovery TakeRecovery { get; private set; }
		public FireunitInteractiveTargetData InteractiveTargetData { get; private set; }


		public override void BaseAwake()
		{
			base.BaseAwake();
			InteractiveInterface = this;
			TakeDamage = this;
			TakeRecovery = this;
			tacticalStateChecker = null;

			UnitData = ThisContainer.GetData<FireunitData>();
			if(ThisContainer.TryGetData<FireunitInteractiveTargetData>(out FireunitInteractiveTargetData interactiveTargetData))
			{
				InteractiveTargetData = interactiveTargetData;
				InteractiveTargetData.Clear();
			}
			else
			{
				InteractiveTargetData = ThisContainer.AddData<FireunitInteractiveTargetData>();
				InteractiveTargetData.Clear();
			}
			if(ThisContainer.TryGetData<FireunitTypeValue>(out var infoValueData))
			{
				TypeValueData = infoValueData;
			}
			else
			{
				TypeValueData = ThisContainer.AddData<FireunitTypeValue>();
			}
			if(ThisContainer.TryGetData<FireunitPlayValue>(out var playValueData))
			{
				PlayValueData = playValueData;
			}
			else
			{
				PlayValueData = ThisContainer.AddData<FireunitPlayValue>();
			}
			if(ThisContainer.TryGetData<FireunitPoseValue>(out var poseValueData))
			{
				PoseValueData = poseValueData;
			}
			else
			{
				PoseValueData = ThisContainer.AddData<FireunitPoseValue>();
			}
			if(ThisContainer.TryGetData<FireunitStateValue>(out var stateValueData))
			{
				StateValueData = stateValueData;
				StateValueData.IsRetire = false;
			}
			else
			{
				StateValueData = ThisContainer.AddData<FireunitStateValue>();
				StateValueData.IsRetire = false;
			}
			if(!ThisContainer.TryGetComponent<IUnitTacticalCombatStateChanger>(out iUnitStateChanger))
			{
				iUnitStateChanger  = ThisContainer.AddComponent<UnitTacticalCombatStateChanger>();
			}
			if(!ThisContainer.TryGetComponent<IUnitTacticalCombatStateUpdate>(out iUnitStateUpdate))
			{
				iUnitStateUpdate = ThisContainer.AddComponent<UnitNoneCombatState>();
			}
			StateValueData.UnitTacticalCombatStateUpdate = iUnitStateUpdate;
			StateValueData.UnitTacticalCombatStateChanger = iUnitStateChanger;
		}
		public override void BaseDestroy()
		{
			InteractiveInterface = null;
			tacticalStateChecker = null;
			UnitData = null;

			if(InteractiveTargetData != null)
			{
				InteractiveTargetData.Dispose();
				InteractiveTargetData = null;
			}
			if(PlayValueData != null)
			{
				PlayValueData.Dispose();
				PlayValueData = null;
			}
			if(StateValueData != null)
			{
				StateValueData = null;
			}
			if(InteractiveTargetData != null)
			{
				InteractiveTargetData.Dispose();
				InteractiveTargetData = null;
			}
		}
	}
}
