using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerInterface
{
	public interface IGamePlayer : IOdccComponent
	{
		void OnSelectFireteam(int selectTeamIndex);
		void OnMovementToAnchor(int movementToAnchor);
	}


	public class PlayerRawInterface : ComponentBehaviour
	{
		private IGamePlayer gamePlayer;

		private bool IsValid => gamePlayer != null;
		private bool IsNotValid => !IsValid;

		public override void BaseEnable()
		{
			base.BaseEnable();
			gamePlayer = ThisContainer.GetComponent<IGamePlayer>();
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
