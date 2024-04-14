namespace BC.GamePlayerManager
{
	public class AIGamePlayer : GamePlayer
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
			playerData.IsAI = true;
		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			playerData.IsAI = true;
		}
	}
}
