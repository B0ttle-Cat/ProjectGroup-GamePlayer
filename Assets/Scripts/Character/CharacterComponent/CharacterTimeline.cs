using BC.ODCC;

using UnityEngine;
using UnityEngine.Playables;

namespace BC.Character
{
	[RequireComponent(typeof(PlayableDirector))]
	public class CharacterTimeline : ComponentBehaviour
	{
		PlayableDirector playable;


		public override void BaseAwake()
		{
			playable = GetComponent<PlayableDirector>();
		}

	}
}
