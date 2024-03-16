namespace BC.GamePlayerManager
{
	public class HumanPlayer : GamePlayer
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(ThisContainer.TryGetData<GamePlayerData>(out var data))
			{
				data.IsAI = false;
			}
		}
	}
}
