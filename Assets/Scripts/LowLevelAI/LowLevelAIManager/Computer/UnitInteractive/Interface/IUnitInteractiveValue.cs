using BC.ODCC;
using BC.OdccBase;

namespace BC.LowLevelAI
{
	/// <summary>
	/// <see cref="UnitInteractiveInfo"/>
	/// </summary>
	public interface IUnitInteractiveValue : IOdccComponent
	{
		public int UniqueID { get => UnitData.MemberUniqueID; }
		IFireunitData UnitData { get; }
		IFindCollectedMembers FindMembers { get; set; }
		IUnitInteractiveComputer Computer { get; set; }
		IUnitPlayValue PlayValueData { get; }
		IUnitPoseValue PoseValueData { get; }
		IUnitStateValue StateValueData { get; }
		IUnitInteractiveInterface InteractiveInterface { get; }
		FireunitInteractiveTargetData InteractiveTargetData { get; }
	}
}
