namespace BC.GamePlayerManager
{
	public class HumanPlayer : GamePlayer
	{
		public override void BaseValidate()
		{
			base.BaseValidate();
			playerData.IsAI = false;
		}
		public override void BaseAwake()
		{
			base.BaseAwake();
			playerData.IsAI = false;
		}
	}
}
