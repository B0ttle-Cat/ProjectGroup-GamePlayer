using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.Map
{
	public class StrategicPoint : ComponentBehaviour, IStrategicPoint
	{
		public int pointPerTime;
		private MapPathPoint thiaPathPoint;
		private StrategicPointData strategicPointData;

		public override void BaseValidate()
		{
			if(!ThisContainer.TryGetData<StrategicPointData>(out strategicPointData))
			{
				strategicPointData = ThisContainer.AddData<StrategicPointData>();
			}

			base.BaseValidate();
		}

		public override void BaseAwake()
		{
			base.BaseAwake();
			if(!ThisContainer.TryGetData<StrategicPointData>(out strategicPointData))
			{
				strategicPointData = ThisContainer.AddData<StrategicPointData>();
			}

			if(ThisContainer.TryGetComponent<MapPathPoint>(out thiaPathPoint))
			{

			}
		}


#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			try
			{
				if(!Application.isPlaying) ThisContainer.ContainerNode.AllRefresh();

				if(ThisContainer.TryGetData<StrategicPointData>(out strategicPointData))
				{
					if(strategicPointData.controlerFaction >= 0)
					{
						Gizmos.DrawSphere(ThisTransform.position + Vector3.up * 10, 1f);
					}
				}
			}
			catch { }
		}
#endif
	}
}
