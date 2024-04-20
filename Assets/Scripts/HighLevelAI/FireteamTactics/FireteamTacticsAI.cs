using System.Collections.Generic;
using System.Linq;

using BC.OdccBase;
using BC.LowLevelAI;
using BC.ODCC;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FireteamTacticsAI : ComponentBehaviour
	{
		private FireteamData fireteamData;
		private FireteamTacticsData tacticsData;
		public List<ObjectBehaviour> fireunitList;

		public override void BaseAwake()
		{
			base.BaseAwake();
			fireteamData = ThisContainer.GetData<FireteamData>();
			tacticsData = ThisContainer.GetData<FireteamTacticsData>();
			fireunitList = new List<ObjectBehaviour>();

			OdccQueryCollector.CreateQueryCollector(
				QuerySystemBuilder.CreateQuery().WithAll<FireunitData, FireunitController, FireunitMovementAgent>().Build())
				.CreateChangeListEvent(InitFireTeamList, UpdateFireTeamList);
		}

		private void InitFireTeamList(IEnumerable<ObjectBehaviour> initList)
		{
			fireunitList = initList
				.Where(item
					=> item.ThisContainer.TryGetData<FireunitData>(out var data)
					&& fireteamData.FactionIndex == data.FactionIndex
					&& fireteamData.TeamIndex == data.TeamIndex)
				.ToList();

			UpdateTacticsPosition();
		}
		private void UpdateFireTeamList(ObjectBehaviour item, bool added)
		{
			if(item.ThisContainer.TryGetData<FireteamData>(out var data)
				&& fireteamData.FactionIndex == data.FactionIndex
				&& fireteamData.TeamIndex == data.TeamIndex)
			{
				if(added)
				{
					fireunitList.Add(item);
				}
				else
				{
					fireunitList.Remove(item);
				}

				UpdateTacticsPosition();
			}
		}

		internal void UpdateTacticsValue()
		{
			UpdateTacticsPosition();
			//UpdatePath();
		}
		internal void UpdateTacticsState()
		{
			//UpdateTacticsStateUsingPath();
		}
		/// <summary>
		/// Fireteam �� �߽��� �Ҽ��ϴ� ���ֵ��� �߽ɿ� ��ġ�ϵ����Ѵ�.
		/// �� ��ġ�� tacticsData �� �����Ѵ�.
		/// </summary>
		private void UpdateTacticsPosition()
		{
			int length = fireunitList.Count;
			Bounds bounds = new Bounds();
			for(int i = 0 ; i < length ; i++)
			{
				var unit = fireunitList[i];
				Vector3 position = unit.ThisTransform.position;

				if(i == 0)
				{
					bounds = new Bounds(position, Vector3.zero);
				}
				else
				{
					bounds.Encapsulate(position);
				}
			}
			tacticsData.tacticsPosition = bounds.center;
			ThisTransform.position = tacticsData.tacticsPosition;
		}
		/// <summary>
		/// tacticsData �� ��θ� �����Ѵ�.
		/// </summary>
		//private void UpdatePath()
		//{
		//	StrategicPath targetPath = tacticsData.TargetPath;
		//	if(targetPath == null)
		//	{
		//		tacticsData.movePath = null;
		//		return;
		//	}
		//	StrategicPath firstPath = tacticsData.movePath;
		//
		//	// ���� ��ǥ�� ���ų� ������ ��ǥ�� �ٸ� ����� �ϴ޵� 
		//	if(firstPath.from.FindTargetPath(targetPath.to, out var pathList, out var totalCost))
		//	{
		//		tacticsData.fullPathList = pathList;
		//	}
		//	else
		//	{
		//		tacticsData.fullPathList.Clear();
		//	}
		//
		//	if(tacticsData.fullPathList.Count == 0)
		//	{
		//		firstPath = null;
		//	}
		//	else
		//	{
		//		firstPath = tacticsData.fullPathList[0];
		//	}
		//
		//	tacticsData.movePath = firstPath;
		//}

		/// <summary>
		/// ���� ���� ���� ������� / ���� ���� �����Ѵ�.
		/// </summary>
		//private void UpdateTacticsStateUsingPath()
		//{
		//	Vector3 tacticsPosition = tacticsData.tacticsPosition;
		//	var tacticsMove = tacticsData.movePath;
		//	if(tacticsMove == null) return;
		//
		//	float StayPoint = tacticsMove.StayPoint;// * tacticsData.variable.stayVariable;
		//	float MovePoint = tacticsMove.MovePoint;// * tacticsData.variable.moveVariable;
		//
		//	if(StayPoint >= MovePoint)
		//	{
		//		// �ӹ���.
		//		tacticsData.movePoint = tacticsMove.from;
		//	}
		//	else
		//	{
		//		// �̵��Ѵ�.
		//		tacticsData.movePoint = tacticsMove.to;
		//	}
		//}
	}
}