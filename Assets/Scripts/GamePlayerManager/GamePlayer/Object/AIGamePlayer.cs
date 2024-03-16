namespace BC.GamePlayerManager
{
	public class AIGamePlayer : GamePlayer
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
			if(ThisContainer.TryGetData<GamePlayerData>(out var data))
			{
				data.IsAI = true;
			}
		}
	}
}
