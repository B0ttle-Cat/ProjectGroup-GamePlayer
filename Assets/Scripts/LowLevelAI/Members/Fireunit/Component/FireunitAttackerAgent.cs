using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	public class FireunitAttackerAgent : ComponentBehaviour, IUnitAttackerAgent
	{
		public bool lockChangeTarget { get; set; }
		public bool hasChange { get; set; }
		public IUnitInteractiveValue ThisActor { get; private set; }
		public IUnitInteractiveValue ThisTarget { get; private set; }
		private IUnitInteractiveValue NextTarget { get; set; }

		public override void BaseAwake()
		{
			ThisTarget = null;
			NextTarget = null;
			hasChange = false;
			lockChangeTarget = false;
			base.BaseAwake();
		}
		void IUnitAttackerAgent.InputAttackTarget(UnitInteractiveInfo info)
		{
			ThisActor = info.Actor;
			NextTarget = info.Target;
		}
		void IUnitAttackerAgent.InputAttackStop()
		{
			ThisActor = null;
			NextTarget = null;
		}
		public override void BaseUpdate()
		{
			base.BaseUpdate();

			CheckUpdateFunction();
			ThisTarget = NextTarget;

			void CheckUpdateFunction()
			{
				if(NextTarget == null && ThisTarget == null)
				{
					hasChange = false;
					NoneTarget();
				}
				else if(NextTarget != null && ThisTarget == null)
				{
					hasChange = true;
					NewTarget();
				}
				else if(NextTarget == null && ThisTarget != null)
				{
					hasChange = true;
					RemoveTarget();
				}
				else if(NextTarget.MemberUniqueID == ThisTarget.MemberUniqueID)
				{
					hasChange = false;
					EqualsTarget();
				}
				else
				{
					hasChange = true;
					ChangeTarget();
				}
			}

			void NoneTarget()
			{

			}
			void NewTarget()
			{

			}
			void RemoveTarget()
			{

			}
			void EqualsTarget()
			{

			}
			void ChangeTarget()
			{

			}
		}
	}
}
