using System.Collections.Generic;
using System.Linq;

using BC.LowLevelAI;
using BC.ODCC;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FactionStrategyAI : ComponentBehaviour
	{
		private FactionData factionData;
		private FactionStrategyData strategyData;
		public List<FireteamTacticsAI> fireteamList;

#if UNITY_EDITOR
		[SerializeField]
		private bool IsOnDrawGizmos;
#endif
		public override void BaseAwake()
		{
			base.BaseAwake();
			factionData = ThisContainer.GetData<FactionData>();
			strategyData = ThisContainer.GetData<FactionStrategyData>();
		}

		public override void BaseEnable()
		{
			base.BaseEnable();
			OdccQueryCollector.CreateQueryCollector(
				QuerySystemBuilder.CreateQuery().WithAll<FireteamObject, FireteamTacticsAI>().Build())
				.CreateChangeListEvent(InitFireTeamList, UpdateFireTeamList);
		}
		public override void BaseDisable()
		{
			base.BaseDisable();

			OdccQueryCollector.CreateQueryCollector(
				QuerySystemBuilder.CreateQuery().WithAll<FireteamObject, FireteamTacticsAI>().Build())
				.DeleteChangeListEvent(UpdateFireTeamList);
		}


		private void InitFireTeamList(IEnumerable<ObjectBehaviour> initList)
		{
			fireteamList = initList
				.Where(item
					=> item.ThisContainer.TryGetData<FireteamData>(out var data)
					&& factionData.FactionIndex == data.FactionIndex)
				.Select(item => item.ThisContainer.GetComponent<FireteamTacticsAI>())
				.Where(item => item != null)
				.ToList();
		}
		private void UpdateFireTeamList(ObjectBehaviour item, bool added)
		{
			if(item.ThisContainer.TryGetData<FireteamData>(out var data) && factionData.FactionIndex == data.FactionIndex
				&& item.ThisContainer.TryGetComponent<FireteamTacticsAI>(out var fireteam))
			{
				if(added)
				{
					fireteamList.Add(fireteam);
				}
				else
				{
					fireteamList.Remove(fireteam);
				}
			}
		}

		//public void UpdateNextStrategicPoint(IEnumerable<StrategicPoint> AllStrategicPoints)
		//{
		//	int thisFactionIndex = factionData.FactionIndex;
		//	strategyData.NextStrategicPath = AllStrategicPoints
		//		.SelectMany(item => item.nextStrategicPoint)
		//		.Where(item =>
		//		{
		//			if(item.from.ThisContainer.TryGetData<FactionData>(out var fromFactionData))
		//			{
		//				if(thisFactionIndex == fromFactionData.FactionIndex)
		//				{
		//					if(item.to.ThisContainer.TryGetData<FactionData>(out var toFactionData))
		//					{
		//						return thisFactionIndex != toFactionData.FactionIndex;
		//					}
		//					else
		//					{
		//						return true;
		//					}
		//				}
		//			}
		//			return false;
		//		})
		//		.ToList();
		//}

		//internal void CalculateNextStrategicPoint()
		//{
		//	if(strategyData.totalCostOfPath == null)
		//		strategyData.totalCostOfPath = new List<float>();
		//	else
		//		strategyData.totalCostOfPath.Clear();
		//
		//	var pathVariable = strategyData.variable;
		//	int length = strategyData.NextStrategicPath.Count;
		//	for(int i = 0 ; i < length ; i++)
		//	{
		//		StrategicPath strategicPath = strategyData.NextStrategicPath[i];
		//
		//		float pathPoint = strategicPath.PointPerTime * pathVariable.pointVariable;// * variable.PointRandom();
		//		float pathCost = strategicPath.DistancePerCost * pathVariable.costVariable;// * variable.CostRandom();
		//
		//		float somethingCost = 0f; // * variable.CostRandom();
		//
		//		float totalCost = pathCost - pathPoint + somethingCost;
		//		strategyData.totalCostOfPath.Add(totalCost);
		//	}
		//}

		//internal void SelectNextStrategicPath()
		//{
		//	var list = strategyData.totalCostOfPath;
		//	strategyData.selectcStrategicPath = list.Select((float value, int index) => (value, index))
		//		.OrderBy(item => item.value)
		//		.Take(strategyData.ResultCount)
		//		.Select(item => strategyData.NextStrategicPath[item.index])
		//		.ToList();
		//}

		//internal void GiveOrderToFireteam()
		//{
		//	List<StrategicPath> updateTargetList = new List<StrategicPath>();
		//	List<FireteamTacticsAI> updateFireteamList = new List<FireteamTacticsAI>();
		//
		//	// �غ�� ��ǥ
		//	updateTargetList.AddRange(strategyData.selectcStrategicPath);
		//	// �غ�� ��
		//	updateFireteamList.AddRange(fireteamList);
		//
		//	// �غ� ��ǥ�� �� �غ� ���� �ִ� ���.
		//	if(updateTargetList.Count > 0 && updateFireteamList.Count > 0)
		//	{
		//		for(int i = 0 ; i < updateFireteamList.Count ; i++)
		//		{
		//			if(updateTargetList.Count > 0)
		//			{
		//				var fireteam = updateFireteamList[i];
		//				var currentOrderTarget = fireteam.GetCurrentOrder();
		//				if(currentOrderTarget != null)
		//				{
		//					// �̹� �������� ��ǥ�� ������
		//					// �غ� ��ǥ�� �غ� ���� ����
		//					updateTargetList.Remove(currentOrderTarget);
		//					updateFireteamList.RemoveAt(i--);
		//				}
		//			}
		//			else
		//			{
		//				break;
		//			}
		//		}
		//	}
		//
		//	// �غ� ��ǥ�� �� �غ� ���� �����ִ� ���.
		//	// �Ҵ�� ��ǥ�� �켱������ ���� ���� ��ǥ�� �� ���� �߰�.
		//	if(updateTargetList.Count > 0 && updateFireteamList.Count > 0)
		//	{
		//		for(int i = 0 ; i < updateFireteamList.Count ; i++)
		//		{
		//			if(updateTargetList.Count > 0)
		//			{
		//				var fireteam = fireteamList[i];
		//				var currentOrderTarget = fireteam.GetCurrentOrder();
		//
		//				if(currentOrderTarget == null)
		//				{
		//					// ��ǥ�� ���� ������ ��ǥ�� �Ҵ� ��
		//					// �غ� ��ǥ�� �غ� ���� ����
		//					var randomIndex = updateTargetList.RandomIndex();
		//					fireteam.TakeOrders(updateTargetList[randomIndex]);
		//					updateTargetList.RemoveAt(randomIndex);
		//					updateFireteamList.RemoveAt(i--);
		//				}
		//			}
		//			else
		//			{
		//				// ��ǥ ����
		//				break;
		//			}
		//		}
		//	}
		//
		//	// �غ� ��ǥ�� ���������� �غ����� �����ִ� ���. (���� �̵�)
		//	if(updateTargetList.Count == 0 && updateFireteamList.Count > 0)
		//	{
		//		List<StrategicPath> targetList = strategyData.selectcStrategicPath;
		//		for(int i = 0 ; i < updateFireteamList.Count ; i++)
		//		{
		//			var fireteam = updateFireteamList[i];
		//			var currentOrderTarget = fireteam.GetCurrentOrder();
		//
		//			if(currentOrderTarget == null)
		//			{
		//				var randomIndex = targetList.RandomIndex();
		//				if(randomIndex > 0)
		//				{
		//					// ��ǥ �й� ��, ���� ��ǥ�� ���� ������, �ٸ����� �Ҵ�� ��ǥ�� �ߺ����� ��ġ
		//					fireteam.TakeOrders(targetList[randomIndex]);
		//				}
		//			}
		//		}
		//	}
		//	// �غ� ��ǥ�� ���� ������ �غ� ���� ���ڶ� ���.
		//	// Stap 1: ������ ��ǥ�� ���� ���� �Ҵ�Ǿ� ������ �ش� ���� ��ǥ ����.
		//	// Stap 2: �Ҵ�� ��ǥ�� �켱������ ���� ���� Ž�� ��ǥ�� ����.
		//	else if(updateTargetList.Count > 0 && updateFireteamList.Count == 0)
		//	{
		//		for(int i = 0 ; i < fireteamList.Count ; i++)
		//		{
		//			var fireteam = fireteamList[i];
		//			var currentOrderTarget = fireteam.GetCurrentOrder();
		//			if(currentOrderTarget != null)
		//			{
		//
		//			}
		//		}
		//	}
		//}


#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if(!IsOnDrawGizmos) return;

			if(ThisContainer == null) return;

			if(ThisContainer.TryGetData<FactionStrategyData>(out var strategyData))
			{
				var pathList = strategyData.NextStrategicPath;

				Vector3 offsetUp = Vector3.up * 5.5f;

				for(int i = 0 ; i < pathList.Count ; i++)
				{
					var path = pathList[i];
					Color color = Color.yellow;
					if(strategyData.selectcStrategicPath.Contains(path))
					{
						color = Color.red;
					}
					Gizmos.color = color;

					if(path.path.Length >= 2)
					{
						for(int ii = 0 ; ii < path.path.Length-1 ; ii++)
						{
							Vector3 point1 = path.path[ii].OnNavMeshPosition + offsetUp;
							Vector3 point2 = path.path[ii+1].OnNavMeshPosition + offsetUp;
							Gizmos.DrawLine(point1, point2);
						}
					}
				}
			}
		}
#endif
	}
}
