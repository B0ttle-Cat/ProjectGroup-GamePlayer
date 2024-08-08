using System.Collections.Generic;

using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class FireunitInteractiveValue : ComponentBehaviour, IUnitInteractiveValue, IUnitInteractiveInterface
	{
		// Data
		public IFireunitData UnitData { get; private set; }
		public IFindCollectedMembers FindMembers { get; set; }
		public IUnitInteractiveComputer Computer { get; set; }
		public IUnitPlayValue PlayValueData { get; private set; }
		public IUnitPoseValue PoseValueData { get; private set; }
		public IUnitInteractiveInterface InteractiveInterface { get; private set; }

		private FireunitInteractiveTargetData interactiveTargetData;
		public bool IsRetire { get; private set; }


		public double buffTimeUpdate;


		public override void BaseAwake()
		{
			base.BaseAwake();
			UnitData = ThisContainer.GetData<FireunitData>();
			if(!ThisContainer.TryGetData<FireunitInteractiveTargetData>(out interactiveTargetData))
			{
				interactiveTargetData = ThisContainer.AddData<FireunitInteractiveTargetData>();
				interactiveTargetData.Clear();
			}
			if(ThisContainer.TryGetData<FireunitPlayValue>(out var playValueData))
			{
				PlayValueData = playValueData;
				PlayValueData.UnitData = UnitData;
			}
			if(ThisContainer.TryGetData<FireunitPoseValue>(out var poseValueData))
			{
				PoseValueData = poseValueData;
				PoseValueData.UnitData = UnitData;
			}
			InteractiveInterface = this;
			IsRetire = false;
		}
		void IUnitInteractiveInterface.InitValueUpdate()
		{
			IsRetire = false;
			interactiveTargetData.Clear();
			buffTimeUpdate = Time.timeAsDouble;
		}
		void IUnitInteractiveInterface.ReleaseValueUpdate()
		{
			if(interactiveTargetData != null)
			{
				interactiveTargetData.Dispose();
				interactiveTargetData = null;
			}
		}

		void IUnitInteractiveInterface.BuffTimeUpdate()
		{
			double deltaTime = Time.timeAsDouble - buffTimeUpdate;
			buffTimeUpdate = Time.timeAsDouble;
			ThisContainer.CallActionAllComponent<IBuffTimeUpdate>(call => {
				call.LifeTimeUpdate(in deltaTime);
			});
		}
		void IUnitInteractiveInterface.UnitHealthUpdate()
		{
			var HealthPoint = PlayValueData.HealthPoint;
			if(HealthPoint.Value <= HealthPoint.Min)
			{
				IsRetire = true;
			}
			else
			{
				IsRetire = false;
			}

			PlayValueData.HealthPoint = HealthPoint;
		}
		void IUnitInteractiveInterface.UnitPoseUpdate()
		{
			PoseValueData.ThisUnitPosition = ThisTransform.position;
			PoseValueData.ThisUnitPosition = ThisTransform.forward;
			PoseValueData.ThisUnitLookUP = ThisTransform.up;
		}


		void IUnitInteractiveInterface.VisualRangeUpdate()
		{
			var VisualRange = PlayValueData.VisualRange;

			VisualRange.Value = 10f;
			VisualRange.Value = Mathf.Clamp(VisualRange.Value, VisualRange.Min, VisualRange.Max);

			PlayValueData.VisualRange = VisualRange;
		}
		void IUnitInteractiveInterface.ActionRangeUpdate()
		{
			var ActionRange = PlayValueData.ActionRange;

			ActionRange.Start = 10f;
			ActionRange.Ended = 12f;

			ActionRange.Start = Mathf.Clamp(ActionRange.Start, ActionRange.Min, ActionRange.Ended);
			ActionRange.Ended = Mathf.Clamp(ActionRange.Ended, ActionRange.Start, ActionRange.Max);

			PlayValueData.ActionRange = ActionRange;
		}

		void IUnitInteractiveInterface.InActionRangeTargetList(List<UnitInteractiveInfo> inActionRangeTargetList)
		{
			// TODO :: ¿Ã¡¶ ¿Ã∞…∑Œ πª«“¡ˆ ∞ÌπŒ«œ∫¡æﬂ «‘.
			interactiveTargetData.NewRange(inActionRangeTargetList);
		}
	}
}
