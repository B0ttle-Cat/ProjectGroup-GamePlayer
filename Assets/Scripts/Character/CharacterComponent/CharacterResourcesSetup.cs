using System;

using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace BC.Character
{
	public class CharacterResourcesSetup : ComponentBehaviour
	{
		[InlineProperty, HideLabel]
		[FoldoutGroup("CharacterModel")]
		public ResourcesKey characterModel;
		[Space]
		[InlineProperty, HideLabel]
		[FoldoutGroup("WeaponModel")]
		public ResourcesKey weaponModel;

		[Space]
		[SerializeField]
		private bool IsDisableWhenReady;
		[ShowIf("@IsDisableWhenReady")]
		[SerializeField]
		private bool IsDestroyWhenReadyAndDisable;

		[SerializeField, ReadOnly]
		private bool IsReady;

		private Action<CharacterModel, WeaponModel> isAllReady;
		[SerializeField, ReadOnly]
		private CharacterModel model = null;
		[SerializeField, ReadOnly]
		private WeaponModel weapon = null;

#if UNITY_EDITOR
		[ContextMenu("Test Set Editor")]
		private void SetEditor()
		{
			ResourcesSetup(null);
		}
#endif
		public void ResourcesSetup(ResourcesKey characterModel, ResourcesKey weaponModel, Action<CharacterModel, WeaponModel> readyCallback)
		{
			this.characterModel = characterModel;
			this.weaponModel = weaponModel;
			ResourcesSetup(readyCallback);
		}
		public void ResourcesSetup(Action<CharacterModel, WeaponModel> readyCallback)
		{
			isAllReady = readyCallback;
		}

		public override void BaseEnable()
		{
			IsReady = false;
			model = ThisContainer.GetComponent<CharacterModel>();
			weapon = ThisContainer.GetComponent<WeaponModel>();

			if(model != null && weapon != null)
			{
				return;
			}

			if(model == null)
			{
				characterModel.AsyncInstantiate<GameObject>(this, thisObject => {
					if(thisObject == null)
					{
						Debug.LogError("CharacterModel Instantiate Is Null");
						return;
					}
					thisObject.transform.ResetLcoalPose(ThisTransform);
					model = thisObject.GetComponent<CharacterModel>();
					thisObject.SetActive(true);
				});
			}
			if(weapon == null)
			{
				weaponModel.AsyncInstantiate<GameObject>(this, thisObject => {
					if(thisObject == null)
					{
						Debug.LogError("WeaponModel Instantiate Is Null");
						return;
					}
					thisObject.transform.ResetLcoalPose(ThisTransform);
					weapon = thisObject.GetComponent<WeaponModel>();
					thisObject.SetActive(true);
				});
			}
		}

		public override void BaseDisable()
		{
			characterModel.Unload(this);
			weaponModel.Unload(this);

			if(IsDisableWhenReady && IsDestroyWhenReadyAndDisable)
			{
				Destroy(this);
			}
		}

		public override void BaseUpdate()
		{
			base.BaseUpdate();

			if(!IsReady && CheckUpdateAllReady())
			{
				IsReady = true;

				isAllReady.Invoke(model, weapon);

				if(IsDisableWhenReady)
				{
					enabled = false;
				}
			}
		}

		private bool CheckUpdateAllReady()
		{
			if(isAllReady == null) return false;
			if(model == null  || weapon == null) return false;

			return true;
		}

	}
}
