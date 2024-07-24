using BC.ODCC;

namespace BC.LowLevelAI
{
	public abstract class MemberInteractiveValue : ComponentBehaviour, IMemberInteractiveValue
	{
		public IMemberInteractiveComputer Computer { get; set; }

		public abstract void OnUpdateInit();
		public abstract void OnValueRefresh();
		public abstract void IsAfterValueUpdate();
	}
}
