using System;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.Character
{
	public class CharacterResourcesSetup : ComponentBehaviour
	{
		[InlineProperty, HideLabel]
		[Header("CharacterModel")]
		public ResourcesKey characterModel;
		[Space]
		[InlineProperty, HideLabel]
		[Header("WeaponModel")]
		public ResourcesKey weaponModel;

#if UNITY_EDITOR
		[ContextMenu("SetEditor")]
		private void SetEditor()
		{
			ResourcesSetup(null);
		}
#endif
		public void ResourcesSetup(Action<CharacterModel, WeaponModel> endSetup)
		{
			CharacterModel model = null;
			WeaponModel weapon = null;
			model = GetComponentInChildren<CharacterModel>(true);
			weapon = GetComponentInChildren<WeaponModel>(true);

			if(model != null && weapon != null)
			{
				CheckSetUp();
				return;
			}

			if(model == null)
			{
				characterModel.AsyncInstantiate<GameObject>(ThisObject =>
				{
					ThisObject.transform.parent = transform;
					ThisObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
					ThisObject.transform.localScale = Vector3.one;
					model = ThisObject.GetComponent<CharacterModel>();
					characterModel.Unload();
					CheckSetUp();
				});
			}
			if(weapon == null)
			{
				weaponModel.AsyncInstantiate<GameObject>(ThisObject =>
				{
					ThisObject.transform.parent = transform;
					ThisObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
					ThisObject.transform.localScale = Vector3.one;
					weapon = ThisObject.GetComponent<WeaponModel>();
					weaponModel.Unload();
					CheckSetUp();
				});
			}


			void CheckSetUp()
			{
				if(model != null && weapon != null)
				{
					endSetup?.Invoke(model, weapon);
				}
			}
		}
	}
}
