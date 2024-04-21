namespace BC.Base
{
	public interface INavTarget<T_ID, T_Value>
	{
		T_Value NavTargetValue { get; }

		public void OnSelect(T_ID id)
		{
			EventManager.Call<ICharacterAgent<T_ID, T_Value>>((item) => item.OnMoveStart(this));
		}
	}


	public interface ICharacterAgent<T_ID, T_Value>
	{
		T_ID NavID { get; }
		public void OnMoveStart(INavTarget<T_ID, T_Value> navTarget);
		public void OnMoveStop();
	}
}
