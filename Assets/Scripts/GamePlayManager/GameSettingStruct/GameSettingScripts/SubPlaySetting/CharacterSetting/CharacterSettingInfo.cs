using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	[Serializable]
	public struct CharacterSettingInfo : IDisposable
	{
		[HideLabel, SuffixLabel("CardName", Overlay = true), EnableIf("showEditor"), HorizontalGroup("View"),VerticalGroup("View/ID"), Multiline(2)]
		public string characterName;

#if UNITY_EDITOR
		[ShowInInspector, VerticalGroup("View/ID"), ToggleLeft, LabelText("    Show Editor")]
		private bool showEditor { get; set; }
		[ShowInInspector, PreviewField(50), HorizontalGroup("View", width: 50), HideLabel]
		private UnityEngine.Object CharacterPreview => ResourcesSetter?.CharacterResourcesKey.ObjectAsset;
		[ShowInInspector, PreviewField(50), HorizontalGroup("View", width: 50), HideLabel]
		private UnityEngine.Object WeaponPreview => ResourcesSetter?.WeaponResourcesKey.ObjectAsset;
#endif

		[InlineProperty,HideLabel,FoldoutGroup("Resources Setter"),ShowIf("showEditor"), HideReferenceObjectPicker]
		public CharacterResourcesSetting ResourcesSetter;
		[InlineProperty,HideLabel,FoldoutGroup("Type Setter"),ShowIf("showEditor"),HideReferenceObjectPicker]
		public CharacterTypeSetting TypeSetter;
		[InlineProperty, HideLabel, FoldoutGroup("Value Setter"), ShowIf("showEditor"), HideReferenceObjectPicker]
		public CharacterValueSetting ValueSetter;

		public void Dispose()
		{
			ResourcesSetter = null;
			TypeSetter = null;
			ValueSetter = null;
		}
	}
}
