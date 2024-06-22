using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireunitCommandActor : ComponentBehaviour
	{

	}
	public abstract class FireunitCommandActor<TD> : FireunitCommandActor where TD : FireunitCommandData
	{
		protected  TD commandData;
		public TD CommandData { get => commandData; protected set => commandData = value; }
		protected override void Disposing()
		{
			base.Disposing();
			commandData = null;
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			ThisContainer.RemoveData<TD>();
		}
		public sealed override async void BaseEnable()
		{
			base.BaseEnable();
			if(ThisContainer.ParentContainer == null)
			{
				Destroy(this);
				return;
			}

			CommandData = await ThisContainer.AwaitGetData<TD>(null, DisableCancelToken);

			if(CommandData != null)
				BaseActorEnable();
		}
		public sealed override void BaseDisable()
		{
			base.BaseDisable();

			if(CommandData != null)
				BaseActorDisable();

			CommandData = null;
		}
		public sealed override void BaseUpdate()
		{
			base.BaseUpdate();

			if(CommandData != null)
				BaseActorUpdate();
		}
		public sealed override void BaseStart()
		{
			base.BaseStart();

			if(CommandData != null)
				BaseActorStart();
		}
		public abstract void BaseActorEnable();
		public abstract void BaseActorDisable();
		public virtual void BaseActorStart() { }
		public virtual void BaseActorUpdate() { }
	}
}
