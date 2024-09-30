using System;

using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.GamePlayerManager
{
	[Serializable]
	public class CharacterSetter : DataObject
	{
		[HideLabel, SuffixLabel("CardName", Overlay = true)]
		public string cardName;

		[InlineProperty,HideLabel,Title("Resources Setter"),HideReferenceObjectPicker]
		public CharacterResourcesSetter ResourcesSetter;
		[InlineProperty,HideLabel,Title("Type Setter"),HideReferenceObjectPicker]
		public CharacterTypeSetter TypeSetter;
		[InlineProperty,HideLabel,Title("Value Setter"),HideReferenceObjectPicker]
		public CharacterValueSetter ValueSetter;

		protected override void Disposing()
		{
			ResourcesSetter = null;
			TypeSetter = null;
			ValueSetter = null;
		}
	}
}
