using BC.ODCC;

namespace BC.LowLevelAI
{
	public class EventCommander : ObjectBehaviour, IOdccUpdate.Late
	{
		public virtual void BaseLateUpdate()
		{
			if(ThisContainer.ComponentList.Length == 0)
			{
				Destroy(this);
			}
		}

		protected sealed override bool BaseAddValidation(ComponentBehaviour behaviour)
		{
			return behaviour is EventCommandActor && SubBaseAddValidation(behaviour);
		}
		protected sealed override bool BaseAddValidation(DataObject data)
		{
			return data is EventCommandData && SubBaseAddValidation(data);
		}
		protected virtual bool SubBaseAddValidation(ComponentBehaviour behaviour)
		{
			return true;
		}
		protected virtual bool SubBaseAddValidation(DataObject data)
		{
			return true;
		}
	}
}
