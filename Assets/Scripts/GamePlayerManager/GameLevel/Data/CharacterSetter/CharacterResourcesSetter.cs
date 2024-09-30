using System;

using BC.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[Serializable]
	public partial class CharacterResourcesSetter
	{
		[SerializeField, HideLabel, FoldoutGroup("Model Resources Character")]
		private ResourcesKey characterKey;
		[SerializeField, HideLabel, FoldoutGroup("Model Resources Weapon")]
		private ResourcesKey weaponKey;

		public ResourcesKey CharacterResourcesKey { get => characterKey; }
		public ResourcesKey WeaponResourcesKey { get => weaponKey; }
	}
}
