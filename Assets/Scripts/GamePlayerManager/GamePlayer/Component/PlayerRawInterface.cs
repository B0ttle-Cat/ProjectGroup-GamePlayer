using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	public class PlayerRawInterface : ComponentBehaviour
	{
		private GamePlayer gamePlayer;

		private bool IsValid => gamePlayer != null;
		private bool IsNotValid => !IsValid;

		public override void BaseEnable()
		{
			base.BaseEnable();
			gamePlayer = ThisContainer.GetComponent<GamePlayer>();
		}


		[InlineButton("OnSelectFireteam")]
		public int selectTeamIndex;
		public void OnSelectFireteam()
		{
			if(IsNotValid) return;
			gamePlayer.OnSelectFireteam(selectTeamIndex);
		}

		[InlineButton("OnMovementToAnchor")]
		public int movementToAnchor;
		public void OnMovementToAnchor()
		{
			if(IsNotValid) return;
			gamePlayer.OnMovementToAnchor(movementToAnchor);
		}
	}
}
