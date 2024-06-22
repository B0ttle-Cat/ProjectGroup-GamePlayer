namespace BC.LowLevelAI
{
	public class FireteamCommandActor : EventCommandActor
	{

	}
	public abstract class FireteamCommandActor<TD> : FireteamCommandActor where TD : FireteamCommandData, new()
	{
		protected FireteamMemberCollector fireteamMembers;
		public FireteamMemberCollector FireteamMembers { get; protected set; }

		protected  TD commandData;
		public TD CommandData { get => commandData; protected set => commandData = value; }

		public sealed override void BaseReset()
		{
			base.BaseReset();
			if(!ThisContainer.TryGetData<TD>(out commandData))
			{
				commandData = ThisContainer.AddData<TD>();
			}

			BaseActorResetAndValidate();
		}
		public sealed override void BaseValidate()
		{
			base.BaseValidate();
			if(!ThisContainer.TryGetData<TD>(out commandData))
			{
				commandData = ThisContainer.AddData<TD>();
			}

			BaseActorResetAndValidate();
		}
		public virtual void BaseActorResetAndValidate() { }
		public sealed override async void BaseEnable()
		{
			base.BaseEnable();
			if(ThisContainer.ParentContainer == null)
			{
				Destroy(this);
				return;
			}

			CommandData = await ThisContainer.AwaitGetData<TD>(null, DisableCancelToken);
			FireteamMembers = CommandData?.Members;

			if(FireteamMembers != null && CommandData != null)
				BaseActorEnable();
		}
		public sealed override void BaseDisable()
		{
			base.BaseDisable();

			if(FireteamMembers != null && CommandData != null)
				BaseActorDisable();

			FireteamMembers = null;
			CommandData = null;
		}
		public abstract void BaseActorEnable();
		public abstract void BaseActorDisable();


		public sealed override void BaseLateUpdate()
		{
			if(FireteamMembers != null && CommandData != null)
				BaseActorLateUpdate();
		}
		public abstract void BaseActorLateUpdate();
	}
}
