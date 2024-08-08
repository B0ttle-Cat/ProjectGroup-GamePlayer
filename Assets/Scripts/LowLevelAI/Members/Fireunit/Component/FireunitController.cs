using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireunitController : ComponentBehaviour//, IFireunitStateTagController
	{
		//[ShowInInspector]
		//private bool IsChangeState { get; set; }

		//public override void BaseAwake()
		//{
		//	base.BaseAwake();
		//	CurrentTags = 0;
		//	IsChangeState = false;
		//}


		//public void AddedStateTag(FireunitStateTag stateTags)
		//{
		//	CurrentTags |= stateTags;
		//	CallAIStateChange();
		//}

		//public void RemoveStateTag(FireunitStateTag stateTags)
		//{
		//	CurrentTags |= stateTags;
		//	CallAIStateChange();
		//}
		//public void OnChangeAIState(FireunitStateTag stateTags)
		//{
		//	CurrentTags = stateTags;
		//	CallAIStateChange();
		//}

		//public override void BaseUpdate()
		//{
		//	base.BaseUpdate();
		//	CallAIStateChange();
		//}

		//public void CallAIStateChange()
		//{
		//	if(IsChangeState)
		//	{
		//		IsChangeState = false;
		//		ThisObject.ThisContainer.CallActionAllComponentInChildObject<IFireunitStateChangeListener>(call => call.FireunitStateChangeListener());
		//	}
		//}
	}
}
