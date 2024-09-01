using BC.ODCC;

namespace BC.OdccBase
{
	/// <summary>
	/// <see cref="UnitInteractiveInfo"/>
	/// </summary>
	public interface IUnitInteractiveValue : IOdccComponent
	{
		public int MemberUniqueID { get => UnitData.MemberUniqueID; }
		public int FactionIndex { get => UnitData.FactionIndex; }
		public int TeamIndex { get => UnitData.TeamIndex; }
		public int UnitIndex { get => UnitData.UnitIndex; }
		IFireunitData UnitData { get; }
		IFindCollectedMembers FindMembers { get; set; }
		IUnitPlayValue PlayValueData { get; }
		IUnitPoseValue PoseValueData { get; }
		IUnitStateValue StateValueData { get; }
		IUnitInteractiveInterface InteractiveInterface { get; }
		FireunitInteractiveTargetData InteractiveTargetData { get; }
	}
}
