using System.Collections.Generic;

using BC.ODCC;

namespace BC.LowLevelAI
{
	public enum FireunitStateTag
	{
		IDLE = 0,
		MOVE,
		ATTACK,

	}

	public interface IFireunitStateControl : IOdccComponent
	{
		public List<FireunitStateTag> CurrentTags { get; }
		public void AddedAIState(params FireunitStateTag[] aiStateTags);
		public void RemoveAIState(params FireunitStateTag[] aiStateTags);
		public void OnChangeAIState(FireunitStateTag[] added, FireunitStateTag[] remove);
	}

	public interface IFireunitStateChangeListener : IOdccComponent
	{
		public void FireunitStateChangeListener();
	}
}
