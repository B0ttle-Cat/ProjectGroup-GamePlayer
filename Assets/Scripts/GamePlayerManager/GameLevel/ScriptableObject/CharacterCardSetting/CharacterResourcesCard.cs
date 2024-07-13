using BC.Base;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "CharacterCard", menuName = "BC/StartSetting/new CharacterResourcesCard")]
	public partial class CharacterResourcesCard : ScriptableObject, ICharacterModelData
	{
		[InlineButton("UpdateAssetName")]
		[HideLabel, SuffixLabel("CharacterName", Overlay = true)]
		[Multiline(2), PropertySpace(SpaceAfter = 20)]
		public string characterName;
		[SerializeField, HideLabel, FoldoutGroup("Model Resources Character")]
		private ResourcesKey characterKey;
		[SerializeField, HideLabel, FoldoutGroup("Model Resources Weapone")]
		private ResourcesKey weaponeKey;


		public ResourcesKey CharacterKey { get => characterKey; }
		public ResourcesKey WeaponeKey { get => weaponeKey; }
	}
}
