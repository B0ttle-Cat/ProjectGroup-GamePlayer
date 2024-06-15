using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using UnityEngine;

namespace BC.LowLevelAI
{
	public abstract class MemberCollectorComponent<T> : ComponentBehaviour where T : MemberObject
	{
		[SerializeField]
		private OdccQueryCollector memberCollector;
		public virtual List<T> ThisMembers { get; set; }
		public int Count => ThisMembers is null ? 0 : ThisMembers.Count;

		public T this[int index] => ThisMembers[index];

		public sealed override void BaseAwake()
		{
			base.BaseAwake();

			QuerySystem memberQuery = null;
			ThisMembers = new List<T>();
			BaseAwake_MemberCollector(ref memberQuery);

			if(memberQuery != null)
			{
				memberCollector = OdccQueryCollector.CreateQueryCollector(memberQuery)
					.CreateChangeListEvent(InitList, UpdateList);
			}
		}
		public sealed override void BaseDestroy()
		{
			base.BaseDestroy();

			BaseDestroy_MemberCollector();

			if(memberCollector != null)
			{
				memberCollector.DeleteChangeListEvent(UpdateList);
				memberCollector = null;
			}

			if(ThisMembers != null)
			{
				ThisMembers.Clear();
				ThisMembers = null;
			}
		}
		protected override void Disposing()
		{
			base.Disposing();
			memberCollector = null;
		}
		private void InitList(IEnumerable<ObjectBehaviour> enumerable)
		{
			ThisMembers = Base_MemberInitList(enumerable.Select(item => item as T));
		}
		private void UpdateList(ObjectBehaviour behaviour, bool isAdded)
		{
			if(behaviour is not T member) return;
			Base_MemberUpdateList(member, isAdded);
		}

		public abstract void BaseAwake_MemberCollector(ref QuerySystem memberCollector);
		public abstract void BaseDestroy_MemberCollector();
		public abstract List<T> Base_MemberInitList(IEnumerable<T> enumerable);
		public abstract void Base_MemberUpdateList(T member, bool isAdded);





		public void Foreach(Action<T> action, Func<T, bool> condition = null)
		{
			if(action == null) return;

			int length = Count;
			for(int i = 0 ; i < length ; i++)
			{
				var member = ThisMembers[i];
				if(condition == null || condition.Invoke(member))
				{
					try
					{
						action.Invoke(member);
					}
					catch(Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
		}
		public void Foreach(Action<T, int> action, Func<T, int, bool> condition = null)
		{
			if(action == null) return;

			int length = Count;
			for(int i = 0 ; i < length ; i++)
			{
				var member = ThisMembers[i];
				if(condition == null || condition.Invoke(member, i))
				{
					try
					{
						action.Invoke(member, i);
					}
					catch(Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
		}
	}
}
