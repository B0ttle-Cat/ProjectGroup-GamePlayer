using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class UnitInteractiveComputer : ComponentBehaviour, IUnitInteractiveComputer
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

		// Actor UniqueID : IUnitInteractiveValue
		private Dictionary<int, IUnitInteractiveValue> unitInteractiveValueList;
		// Actor UniqueID : Target UniqueID : IUnitInteractiveValue
		private Dictionary<int, Dictionary<int, UnitInteractiveInfo>> computingList;

		// Actor FactionID : Target UniqueID 
		private Dictionary<int, HashSet<int>> inRangeFactionVisual;

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

			unitInteractiveValueList = new Dictionary<int, IUnitInteractiveValue>();

			computingList = new Dictionary<int, Dictionary<int, UnitInteractiveInfo>>();

			ThisContainer.NextGetData<DiplomacyData>((_data) => {
				diplomacyData = _data;

				valueListCompute = OdccQueryCollector.CreateQueryCollector(computeValueQuery, this)
					.CreateChangeListEvent(InitValueList, UpdateValueList)
					.CreateLooperEvent(nameof(valueListCompute), -2)
					// ������ �����Ӵ� ���Ѽӵ� ������Ʈ
					.CallNext(LooperLimitTimeUpdate)
					// ó�� �ؾ��ϴ� ����Ʈ�� ������Ʈ ������
					.CallNext(AfterValueListUpdate)

					// ������ ���� �ð��� ������Ʈ
					// ����� ������ ����
					.CallNext(BuffTimeUpdate)
					// ä���� ������Ʈ
					// ä���� ������ ��Ÿ�̾�
					.CallNext(HealthPointUpdate)

					// ������ Transform�� ������Ʈ�ϴ� �񵿱� �޼���
					.CallNext(UnitPoseUpdate)

					// ������ ��ȣ�ۿ� �Ÿ��� ���� ���� ������Ʈ
					.CallNext(VisualRangeUpdate)
					.CallNext(InRangeVisualUpdate)
					.CallNext(ActionRangeUpdate)
					.CallNext(InRangeActionUpdate)
					.CallNext(AttackRangeUpdate)
					.CallNext(InRangeAttackUpdate)

					// ������ ���� ���¸� ������Ʈ
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
				var inList = new Dictionary<int, UnitInteractiveInfo>();
				foreach(var targetItem in unitInteractiveValueList)
				{
					var target = targetItem.Value;

					if(actor.UniqueID == target.UniqueID) continue;

					var newInteractiveInfo = new UnitInteractiveInfo(actor, target, ComputeDiplomacyType(actor, target));
					inList.Add(target.UniqueID, newInteractiveInfo);
				}
				computingList.Add(actor.UniqueID, inList);

				actor.FindMembers = FindMembers;
				actor.Computer = this;
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
						int memberUniqueID = value.UnitData.MemberUniqueID;
						value.FindMembers = FindMembers;
						value.Computer = this;
						AddValueComputingList(value);
						unitInteractiveValueList.Add(memberUniqueID, value);
					}
				}
				else
				{
					if(behaviour.ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var value))
					{
						int memberUniqueID = value.UnitData.MemberUniqueID;
						RemoveValueComputingList(value);
						unitInteractiveValueList.Remove(memberUniqueID);
					}
				}
			}

			void AddValueComputingList(IUnitInteractiveValue addValue)
			{
				int addUniqueID = addValue.UnitData.MemberUniqueID;
				var addList = new Dictionary<int, UnitInteractiveInfo>();
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
				int deleteUniqueID = deleteValue.UnitData.MemberUniqueID;
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
		public bool TryUnitTargetList(IUnitInteractiveValue actor, out Dictionary<int, UnitInteractiveInfo> targetToList)
		{
			if(computingList.TryGetValue(actor.UniqueID, out targetToList))
			{
				return true;
			}
			targetToList = null;
			return false;
		}
		public bool TryUnitTargetInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, out UnitInteractiveInfo info)
		{
			if(computingList.TryGetValue(actor.UniqueID, out var inList) && inList.TryGetValue(target.UniqueID, out info))
			{
				return true;
			}
			info = null;
			return false;
		}
		#endregion
	}
}
