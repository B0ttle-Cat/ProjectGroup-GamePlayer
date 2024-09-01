using BC.Base;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.Character
{
	public class WeaponData : DataObject
	{
		public int MaxAttackCount;

		[SerializeField, HideLabel]
		[FoldoutGroup("WeaponeModel")]
		private ResourcesKey weaponeKey;
		public ResourcesKey WeaponResourcesKey { get => weaponeKey; set => weaponeKey = value; }

	}
}
