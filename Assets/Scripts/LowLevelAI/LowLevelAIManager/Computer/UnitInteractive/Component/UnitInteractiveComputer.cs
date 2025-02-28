using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class UnitInteractiveComputer : ComponentBehaviour
	{
		public IFindCollectedMembers FindMembers { get; set; }

		private DiplomacyData diplomacyData;

		[SerializeField]
		private OdccQueryCollector valueListCompute;
		private Queue<Action> afterValueListUpdate;
		private DateTime limitStartTime = DateTime.Now;
		private double limitTime = 0.005;

		[Space]
		[ShowInInspector, ReadOnly]
		// Actor MemberUniqueID : IUnitInteractiveValue
		private Dictionary<Vector3Int, IUnitInteractiveValue> unitInteractiveValueList;
		// Actor MemberUniqueID : Target MemberUniqueID : IUnitInteractiveValue
		private Dictionary<Vector3Int, Dictionary<Vector3Int, UnitInteractiveInfo>> computingList;
		// Actor FactionID : Target MemberUniqueID 
		private Dictionary<int, HashSet<Vector3Int>> inRangeFactionVisual;

		public override void BaseAwake()
		{
			if(ThisContainer.TryGetComponent<IFindCollectedMembers>(out var findCollectedMembers))
			{
				FindMembers = findCollectedMembers;
			}

			var computeValueQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<IUnitInteractiveValue>()
					.Build();

			afterValueListUpdate = new Queue<Action>();

			unitInteractiveValueList = new Dictionary<Vector3Int, IUnitInteractiveValue>();

			computingList = new Dictionary<Vector3Int, Dictionary<Vector3Int, UnitInteractiveInfo>>();

			inRangeFactionVisual = new Dictionary<int, HashSet<Vector3Int>>();

			ThisContainer.NextGetData<DiplomacyData>((_data) => {
				diplomacyData = _data;

				valueListCompute = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
					.CreateChangeListEvent(InitValueList, UpdateValueList)
					.CreateLooperEvent(nameof(valueListCompute), -2)
					// 루퍼의 프레임당 제한속도 업데이트
					.CallNext(LooperLimitTimeUpdate)
					// 처리 해야하는 리스트를 업데이트 시켜줌
					.CallNext(AfterValueListUpdate)

					// 버프의 남은 시간을 업데이트
					// 종료된 버프는 삭제
					.CallNext(BuffTimeUpdate)
					// 채력을 업데이트
					// 채력이 없으면 리타이어
					.CallNext(HealthPointUpdate)

					// 유닛의 Transform을 업데이트하는 비동기 메서드
					.CallNext(UnitPoseUpdate)

					// 유닛의 상호작용 거리에 대한 값을 업데이트
					.CallNext(VisualRangeUpdate)
					.CallNext(InRangeVisualUpdate)
					.CallNext(ActionRangeUpdate)
					.CallNext(InRangeActionUpdate)
					.CallNext(AttackRangeUpdate)
					.CallNext(InRangeAttackUpdate)

					// 유닛의 전투 상태를 업데이트
					.CallNext(TacticalCombatStateUpdate)


					.GetCollector();
			});
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(valueListCompute != null)
			{
				valueListCompute.DeleteLooperEvent(nameof(valueListCompute));
				valueListCompute = null;
			}

			if(computingList != null)
			{
				computingList.Clear();
				computingList = null;
			}
			if(unitInteractiveValueList != null)
			{
				unitInteractiveValueList.Clear();
				unitInteractiveValueList = null;
			}

			if(afterValueListUpdate == null)
			{
				afterValueListUpdate.Clear();
				afterValueListUpdate = null;
			}
		}

		#region CreateChangeListEvent
		private void InitValueList(IEnumerable<ObjectBehaviour> enumerable)
		{
			unitInteractiveValueList.Clear();
			computingList.Clear();

			unitInteractiveValueList = enumerable.Select(item => item.ThisContainer.GetComponent<IUnitInteractiveValue>())
				.Where(item => item != null)
				.ToDictionary(item => item.UnitData.MemberUniqueID);

			int valueLength = unitInteractiveValueList.Count;
			foreach(var actorItem in unitInteractiveValueList)
			{
				var actor = actorItem.Value;
				var inList = new Dictionary<Vector3Int, UnitInteractiveInfo>();
				foreach(var targetItem in unitInteractiveValueList)
				{
					var target = targetItem.Value;

					if(actor.MemberUniqueID == target.MemberUniqueID) continue;

					var newInteractiveInfo = new UnitInteractiveInfo(actor, target, ComputeDiplomacyType(actor, target));
					inList.Add(target.MemberUniqueID, newInteractiveInfo);
				}
				computingList.Add(actor.MemberUniqueID, inList);

				actor.FindMembers = FindMembers;
			}
		}
		private void UpdateValueList(ObjectBehaviour behaviour, bool added)
		{
			if(afterValueListUpdate != null)
			{
				afterValueListUpdate.Enqueue(() => _UpdateList(behaviour, added));
			}
			else
			{
				_UpdateList(behaviour, added);
			}
			void _UpdateList(ObjectBehaviour behaviour, bool added)
			{
				if(added)
				{
					if(behaviour.ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var value))
					{
						Vector3Int memberUniqueID = value.UnitData.MemberUniqueID;
						value.FindMembers = FindMembers;
						AddValueComputingList(value);
						unitInteractiveValueList.Add(memberUniqueID, value);
					}
				}
				else
				{
					if(behaviour.ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var value))
					{
						Vector3Int memberUniqueID = value.UnitData.MemberUniqueID;
						RemoveValueComputingList(value);
						unitInteractiveValueList.Remove(memberUniqueID);
					}
				}
			}

			void AddValueComputingList(IUnitInteractiveValue addValue)
			{
				Vector3Int addUniqueID = addValue.UnitData.MemberUniqueID;
				var addList = new Dictionary<Vector3Int, UnitInteractiveInfo>();
				foreach(var computingItem in computingList)
				{
					var actorUniqueID = computingItem.Key;
					var keyList = computingItem.Value;
					if(actorUniqueID == addValue.UnitData.MemberUniqueID) continue;
					if(!unitInteractiveValueList.TryGetValue(actorUniqueID, out var actorValue)) continue;

					var addInteractiveInfo = new UnitInteractiveInfo(actorValue, addValue, ComputeDiplomacyType(actorValue, addValue));
					var keyInteractiveInfo = new UnitInteractiveInfo(addValue, actorValue, ComputeDiplomacyType(addValue, actorValue));

					keyList.Add(addUniqueID, addInteractiveInfo);
					addList.Add(actorUniqueID, keyInteractiveInfo);
				}
				computingList.Add(addUniqueID, addList);
			}
			void RemoveValueComputingList(IUnitInteractiveValue deleteValue)
			{
				Vector3Int deleteUniqueID = deleteValue.UnitData.MemberUniqueID;
				computingList.Remove(deleteUniqueID);
				foreach(var computingItem in computingList)
				{
					var dic = computingItem.Value;

					dic.Remove(deleteUniqueID);
				}
			}
		}
		#endregion

		#region IUnitInteractiveComputer
		private FactionDiplomacyType ComputeDiplomacyType(IUnitInteractiveValue actor, IUnitInteractiveValue target)
		{
			return diplomacyData.diplomacyTypeList.TryGetValue((actor.UnitData.FactionIndex, target.UnitData.FactionIndex), out var diplomacyType)
				? diplomacyType
				: FactionDiplomacyType.Neutral_Faction;
		}
		#endregion
	}
}
