using System;
using System.Collections;

using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayerManager
{
	[Serializable]
	public struct StartUnitSettingPosition : IFireunitData
	{
		private Vector3Int fireunitData;
		private int factionIndex;
		private int fireteamIndex;
		private int fireunitIndex;

		public Vector3Int MemberUniqueID => fireunitData;
		[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetFactionName")]
		[HideLabel, SuffixLabel("faction", Overlay = true)]
		[GUIColor("DoubleColor")]
		public int FactionIndex { get => fireunitData.x; set => fireunitData.x = value; }
		[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetTeamIndex")]
		[HideLabel, SuffixLabel("team", Overlay = true)]
		[GUIColor("DoubleColor")]
		public int TeamIndex { get => fireunitData.y; set => fireunitData.y = value; }
		[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetUnitIndex")]
		[HideLabel, SuffixLabel("unit", Overlay = true)]
		[GUIColor("DoubleColor")]
		public int UnitIndex { get => fireunitData.z; set => fireunitData.z = value; }

		[HorizontalGroup("ITransformPose"), SerializeField]
		private Vector3 position;
		[HorizontalGroup("ITransformPose"), SerializeField]
		private Vector3 rotation;

		//[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetFactionName")]
		//[HideLabel, SuffixLabel("faction", Overlay = true)]
		//[GUIColor("DoubleColor")]
		//private int factionIndex;
		//[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetTeamIndex")]
		//[HideLabel, SuffixLabel("team", Overlay = true)]
		//[GUIColor("DoubleColor")]
		//private int teamIndex;
		//[HorizontalGroup, SerializeField, ValueDropdown("ShowTargetUnitIndex")]
		//[HideLabel, SuffixLabel("unit", Overlay = true)]
		//[GUIColor("DoubleColor")]
		//private int unitIndex;
		//
		//
		//[HorizontalGroup("ITransformPose"), SerializeField]
		//private Vector3 position;
		//[HorizontalGroup("ITransformPose"), SerializeField]
		//private Vector3 rotation;
		//
		//public int MemberUniqueID { get => 1000000 + (FactionIndex * 010000) + (TeamIndex * 000100) + (UnitIndex); }
		//public int UnitIndex { get => unitIndex; set => unitIndex = value; }
		//public int TeamIndex { get => teamIndex; set => teamIndex = value; }
		//public int FactionIndex { get => factionIndex; set => factionIndex = value; }

		public void Dispose()
		{

		}

#if UNITY_EDITOR
		private bool IsDouble { get; set; }
		private Color DoubleColor() { return IsDouble ? Color.red : Color.white; }
		private static IEnumerable ShowTargetFactionName()
		{
			return null;// FriendshipItem.ShowTargetFactionName();
		}
		private static IEnumerable ShowTargetTeamIndex()
		{
			var result = new ValueDropdownList<int>();
			for(int i = 0 ; i < 100 ; i++)
			{
				result.Add(i.ToString(), i);
			}
			return result;
		}
		private static IEnumerable ShowTargetUnitIndex()
		{
			var result = new ValueDropdownList<int>();
			for(int i = 0 ; i < 10 ; i++)
			{
				result.Add(i.ToString(), i);
			}
			return result;
		}
		internal void SetDouble(bool _isDouble)
		{
			IsDouble = _isDouble;
		}
#endif
	}
}
