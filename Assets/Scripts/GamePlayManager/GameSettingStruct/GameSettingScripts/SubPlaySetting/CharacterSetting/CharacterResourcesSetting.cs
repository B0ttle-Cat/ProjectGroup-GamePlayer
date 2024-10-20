using System;

using BC.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	[Serializable]
	public partial class CharacterResourcesSetting
	{
		[SerializeField, HideLabel]
		private ResourcesKey characterKey;
		[SerializeField, HideLabel]
		private ResourcesKey weaponKey;

		public ResourcesKey CharacterResourcesKey { get => characterKey; }
		public ResourcesKey WeaponResourcesKey { get => weaponKey; }
	}
}
