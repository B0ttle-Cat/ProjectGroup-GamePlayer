using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.LowLevelAI
{
	public class DetectorComputer : ComponentBehaviour
	{
		public IFindCollectedMembers findCollectedMembers;

		public IUnitInteractiveComputer     unitInteractiveComputer;
		public ITeamInteractiveComputer     teamInteractiveComputer;
		public IFactionInteractiveComputer  factionInteractiveComputer;

		private OdccQueryCollector detectorComputer;

		[SerializeField] private OdccQueryCollector fireunitDetector;
		[SerializeField] private OdccQueryCollector fireteamDetector;
		[SerializeField] private OdccQueryCollector factionDetector;
		public override void BaseAwake()
		{
			base.BaseAwake();
			if(ThisContainer.TryGetComponent<IFindCollectedMembers>(out findCollectedMembers)
				&& ThisContainer.TryGetComponent<IFactionInteractiveComputer>(out factionInteractiveComputer)
				&& ThisContainer.TryGetComponent<ITeamInteractiveComputer>(out teamInteractiveComputer)
				&& ThisContainer.TryGetComponent<IUnitInteractiveComputer>(out unitInteractiveComputer))
			{
				var fireunitQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<FireunitObject, IUnitInteractiveValue>()
					.Build(ThisObject, QuerySystem.RangeType.Child);

				var fireteamQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<FireteamObject, ITeamInteractiveValue>()
					.Build(ThisObject, QuerySystem.RangeType.Child);

				var factionQuery = QuerySystemBuilder.CreateQuery()
					.WithAll<FactionObject, IFactionInteractiveValue>()
					.Build(ThisObject, QuerySystem.RangeType.Child);


				fireunitDetector = OdccQueryCollector.CreateQueryCollector(fireunitQuery, this)
					.CreateActionEvent(nameof(fireunitDetector), out var fireunitLooper)
					.Foreach<FireunitObject, IUnitInteractiveValue>(UpdateFireunitDetector)
					.GetCollector();

				fireteamDetector = OdccQueryCollector.CreateQueryCollector(fireteamQuery, this)
					.CreateActionEvent(nameof(fireunitDetector), out var fireteamLooper)
					.Foreach<FireteamObject, ITeamInteractiveValue>(UpdateFireteamDetector)
					.GetCollector();

				factionDetector = OdccQueryCollector.CreateQueryCollector(factionQuery, this)
					.CreateActionEvent(nameof(fireunitDetector), out var factionLooper)
					.Foreach<FactionObject, IFactionInteractiveValue>(UpdateFactionDetector)
					.GetCollector();


				detectorComputer = OdccQueryCollector.CreateQueryCollector(QuerySystemBuilder.CreateQuery().Build(), this)
					.CreateLooperEvent(nameof(detectorComputer), -1)
					.JoinNext(fireunitLooper)
					.JoinNext(fireteamLooper)
					.JoinNext(factionLooper)
					.GetCollector();
			}
		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();

			findCollectedMembers = null;

			unitInteractiveComputer = null;
			teamInteractiveComputer = null;
			factionInteractiveComputer = null;

			fireunitDetector?.DeleteLooperEvent(nameof(fireunitDetector));
			fireteamDetector?.DeleteLooperEvent(nameof(fireteamDetector));
			factionDetector?.DeleteLooperEvent(nameof(factionDetector));
			detectorComputer?.DeleteLooperEvent(nameof(detectorComputer));

			fireunitDetector = null;
			fireteamDetector = null;
			factionDetector = null;
			detectorComputer = null;
		}


		private async Awaitable UpdateFireunitDetector(OdccQueryLooper.LoopInfo loopInfo, FireunitObject unit, IUnitInteractiveValue actorValue)
		{
			if(!unitInteractiveComputer.TryUnitTargetList(actorValue, out var targetToList)) return;

			foreach(var target in targetToList)
			{
				if(loopInfo.isLooperBreak()) return;

				UpdateInfo(actorValue, target.Key, target.Value);
			}
			void UpdateInfo(IUnitInteractiveValue actor, IUnitInteractiveValue target, UnitInteractiveInfo info)
			{
				float A2T_Distance = info.Distance;

				float deltaVisualRange = actor.ClampVisualRange(actor.ThisVisualRange - target.ThisAntiVisualRange);
				float deltaActionRange = actor.ThisActionRange;
				float deltaAttackRange = actor.ClampVisualRange(actor.ThisAttackRange - target.ThisAntiAttackRange);

				info.IsInVisualRange = A2T_Distance < deltaVisualRange;
				info.IsInActionRange = info.IsInVisualRange && A2T_Distance < deltaActionRange;
				info.IsInAttackRange = info.IsInActionRange && A2T_Distance < deltaAttackRange;
			}
		}
		private async Awaitable UpdateFireteamDetector(OdccQueryLooper.LoopInfo loopInfo, FireteamObject unit, ITeamInteractiveValue actorValue)
		{
			if(!teamInteractiveComputer.TryTeamTargetList(actorValue, out var targetToList)) return;

			foreach(var target in targetToList)
			{
				if(loopInfo.isLooperBreak()) return;
				UpdateInfo(actorValue, target.Key, target.Value);
			}
			void UpdateInfo(ITeamInteractiveValue actor, ITeamInteractiveValue target, TeamInteractiveInfo info)
			{
				//var actorMembers = actor.MemberCollector.ThisMembers;
				//var targetMembers = actor.MemberCollector.ThisMembers;
				//
				//for(int i = 0 ; i<actorMembers.Count ; i++)
				//{
				//	if(!actorMembers[i].ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var actorValue)) continue;
				//	for(int ii = 0 ; ii<targetMembers.Count ; ii++)
				//	{
				//		if(!targetMembers[ii].ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var targetValue)) continue;
				//		if(!unitInteractiveComputer.TryUnitTargetInfo(actorValue, targetValue, out var targetInfo))
				//		{
				//			//TODO :: 여기서 두 팀간 관계를 작업
				//		}
				//	}
				//}
			}
		}
		private async Awaitable UpdateFactionDetector(OdccQueryLooper.LoopInfo loopInfo, FactionObject unit, IFactionInteractiveValue actorValue)
		{
			if(!factionInteractiveComputer.TryFactionTargetList(actorValue, out var targetToList)) return;
			foreach(var target in targetToList)
			{
				if(loopInfo.isLooperBreak()) return;
				UpdateInfo(actorValue, target.Key, target.Value);
			}
			void UpdateInfo(IFactionInteractiveValue actor, IFactionInteractiveValue target, FactionInteractiveInfo info)
			{
				//var actorMembers = actor.MemberCollector.ThisMembers;
				//var targetMembers = actor.MemberCollector.ThisMembers;
				//
				//for(int i = 0 ; i<actorMembers.Count ; i++)
				//{
				//	if(!actorMembers[i].ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var actorValue)) continue;
				//	for(int ii = 0 ; ii<targetMembers.Count ; ii++)
				//	{
				//		if(!targetMembers[ii].ThisContainer.TryGetComponent<IUnitInteractiveValue>(out var targetValue)) continue;
				//		if(!unitInteractiveComputer.TryUnitTargetInfo(actorValue, targetValue, out var targetInfo))
				//		{
				//			//TODO :: 여기서 두 세력간 관계를 작업
				//		}
				//	}
				//}
			}
		}
	}
}
