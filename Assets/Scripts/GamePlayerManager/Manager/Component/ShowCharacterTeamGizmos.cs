using System;
using System.Collections.Generic;

using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

using static BC.GamePlayerManager.StartGameSetting;

namespace BC.GamePlayerManager
{
	public class ShowCharacterTeamGizmos : ComponentBehaviour
	{
		public bool onShowGizmos;

		private QuerySystem teamQuerySystem;
		private OdccQueryCollector teamQueryCollector;
		private List<GizmosInfo> teamGizmosINfo;

		private Queue<Action> drawGizmos;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			teamQuerySystem = null;
			teamQueryCollector = null;

			teamGizmosINfo = null;
			drawGizmos = null;
		}

		public override void BaseAwake()
		{
			base.BaseAwake();

			teamQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<FireteamObject, FireteamMembers, IFireteamData>().Build();

			teamQueryCollector = OdccQueryCollector.CreateQueryCollector(teamQuerySystem)
				.CreateLooperEvent(nameof(OnSyncTeamToGizmos), false)
				.Foreach<FireteamMembers, IFireteamData>(OnSyncTeamToGizmos)
				.IsBreakFunction(() => !onShowGizmos)
				.GetCollector();

			teamGizmosINfo = new List<GizmosInfo>();
			if(ThisContainer.TryGetData<StartLevelData>(out var data) && data.gameSetting != null)
			{
				teamGizmosINfo.AddRange(data.gameSetting.TeamGizmosInfo);
			}
			drawGizmos = new Queue<Action>();
		}

		public override void BaseDestroy()
		{
			base.BaseDestroy();
			if(teamQueryCollector != null)
			{
				teamQueryCollector.DeleteLooperEvent(nameof(OnSyncTeamToGizmos));
				teamQueryCollector = null;
			}
			teamQuerySystem = null;

			if(teamGizmosINfo != null)
				teamGizmosINfo.Clear();
			teamGizmosINfo = null;
		}



		private void OnSyncTeamToGizmos(OdccQueryLooper.LoopInfo loopInfo, FireteamMembers member, IFireteamData data)
		{
			Vector3 centerPosition = member.CenterPosition;
			int findGizmos = teamGizmosINfo.FindIndex(item => item.factionIndex == data.FactionIndex && item.teamIndex ==data.TeamIndex);
			if(findGizmos < 0) return;
			GizmosInfo gizmosInfo = teamGizmosINfo[findGizmos];

			Color color = gizmosInfo.gizmoColor;
			Vector3 center = centerPosition + Vector3.up * 2f;
			List<Vector3> memberPos = new List<Vector3>();
			member.Foreach((unit) => {
				memberPos.Add(unit.transform.position + Vector3.up * 2f);
			});

			drawGizmos.Enqueue(() => {
				Gizmos.color = color;
				Gizmos.DrawSphere(center, 0.5f);
				memberPos.ForEach((unitPos) => {
					Gizmos.DrawSphere(unitPos, 0.25f);
				});
			});
			drawGizmos.Enqueue(() => {
				Gizmos.color = color;
				var matrix = Gizmos.matrix;
				memberPos.ForEach((unitPos) => {
					Vector3 cubePos = Vector3.Lerp(center,unitPos,0.5f);
					Vector3 diraction = (center - unitPos).normalized;
					float size = (center - unitPos).magnitude;

					Matrix4x4 rotationMatrix = Matrix4x4.TRS(cubePos, Quaternion.LookRotation(diraction,Vector3.up), Vector3.one);
					Gizmos.matrix = rotationMatrix;
					Gizmos.DrawCube(Vector3.zero, new Vector3(0.2f, 0.01f, size));
				});
				Gizmos.matrix = matrix;
			});
		}


		public void OnDrawGizmos()
		{
			if(!onShowGizmos) return;
			if(drawGizmos == null) return;
			while(drawGizmos.Count > 0)
			{
				try
				{
					drawGizmos.Dequeue().Invoke();
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}
	}
}
