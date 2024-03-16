using System.Collections.Generic;

using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.LowLevelAI
{
	public class FireunitController : ComponentBehaviour, IFireunitStateControl
	{
		[ShowInInspector]
		public List<FireunitStateTag> CurrentTags { get; set; } = new List<FireunitStateTag>();

		[ShowInInspector]
		private bool IsChangeState { get; set; }

		public override void BaseAwake()
		{
			base.BaseAwake();
			CurrentTags = new List<FireunitStateTag>();
			IsChangeState = false;
		}


		public void AddedAIState(params FireunitStateTag[] aiStateTags)
		{
			int count = aiStateTags.Length;
			for(int i = 0 ; i < count ; i++)
			{
				var tag = aiStateTags[i];
				if(!CurrentTags.Contains(tag))
				{
					CurrentTags.Add(tag);
					IsChangeState = true;
				}
			}
			CallAIStateChange();
		}

		public void RemoveAIState(params FireunitStateTag[] aiStateTags)
		{
			int count = aiStateTags.Length;
			for(int i = 0 ; i < count ; i++)
			{
				var tag = aiStateTags[i];
				if(CurrentTags.Remove(tag))
				{
					IsChangeState = true;
				}
			}
			CallAIStateChange();
		}
		public void OnChangeAIState(FireunitStateTag[] added, FireunitStateTag[] remove)
		{
			{
				int count = added.Length;
				for(int i = 0 ; i < count ; i++)
				{
					var tag = added[i];
					if(!CurrentTags.Contains(tag))
					{
						CurrentTags.Add(tag);
						IsChangeState = true;
					}
				}
			}
			{
				int count = remove.Length;
				for(int i = 0 ; i < count ; i++)
				{
					var tag = remove[i];
					if(CurrentTags.Remove(tag))
					{
						IsChangeState = true;
					}
				}
			}
			CallAIStateChange();
		}

		public override void BaseUpdate()
		{
			base.BaseUpdate();
			CallAIStateChange();
		}

		public void CallAIStateChange()
		{
			if(IsChangeState)
			{
				IsChangeState = false;
				ThisObject.ThisContainer.CallActionAllComponentInChildObject<IFireunitStateChangeListener>(call => call.FireunitStateChangeListener());
			}
		}
	}
}
