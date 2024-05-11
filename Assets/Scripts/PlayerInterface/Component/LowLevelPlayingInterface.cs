using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerInterface
{
	public interface IGamePlayingInterface : IOdccComponent
	{
		void OnSelectFireteam(int selectTeamIndex);
		void OnSetMoveTarget(int anchorIndex, int? selectTeamIndex = null);
		void OnTeamSpawnTarget(int anchorIndex, int? selectTeamIndex = null);
		void OnUnitSpawnIndex(int unitIndex, int? selectTeamIndex = null);
	}


	public class LowLevelPlayingInterface : ComponentBehaviour
	{
		private IGamePlayingInterface interfaceReceiver;

		private bool IsValid => interfaceReceiver != null;
		private bool IsNotValid => !IsValid;

		public override void BaseEnable()
		{
			base.BaseEnable();
			interfaceReceiver = ThisContainer.GetComponent<IGamePlayingInterface>();
		}


		[InlineButton("OnSelectFireteam")]
		public int selectTeamIndex;
		public void OnSelectFireteam()
		{
			if(IsNotValid) return;
			interfaceReceiver.OnSelectFireteam(selectTeamIndex);
		}

		[InlineButton("OnSetMoveTarget")]
		public int movementToAnchor;
		public void OnSetMoveTarget()
		{
			if(IsNotValid) return;
			interfaceReceiver.OnSetMoveTarget(movementToAnchor, selectTeamIndex);
		}


		[InlineButton("OnUnitSpawnIndex")]
		public int unitSpawnIndex;
		public void OnUnitSpawnIndex()
		{
			if(IsNotValid) return;
			interfaceReceiver.OnUnitSpawnIndex(unitSpawnIndex, selectTeamIndex);
		}
	}
}
