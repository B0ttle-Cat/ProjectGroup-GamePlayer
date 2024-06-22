using UnityEngine;

namespace BC.LowLevelAI
{
	public static class GetAroundPosition
	{
		public static Vector3 GetAroundMovePosition(int unitIndex, int positionCount, float radius, float radiusRandomOffset, Vector3 moveDiraction)
		{
			if(positionCount == 0 || positionCount == 1)
			{
				return Vector3.zero;
			}
			moveDiraction = moveDiraction.normalized;

			float splitAngle = 360f / positionCount;
			float angle = splitAngle * (unitIndex + Random.Range(-0.45f,0.45f));
			Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * moveDiraction;
			return direction *  (radius + Random.Range(-radiusRandomOffset, 0f));
		}
		public static Vector3[] GetAroundMovePosition(int positionCount, float radius, float radiusRandomOffset, Vector3 moveDiraction)
		{
			if(positionCount == 0 || positionCount == 1)
			{
				return new Vector3[1] { Vector3.zero };
			}
			moveDiraction = moveDiraction.normalized;

			Vector3[] points = new Vector3[positionCount];
			float splitAngle = 360f / positionCount;

			for(int i = 0 ; i < positionCount ; i++)
			{
				float angle = splitAngle * (i + Random.Range(-0.45f,0.45f));
				Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * moveDiraction;
				Vector3 point = direction * (radius + Random.Range(-radiusRandomOffset, 0f));

				points[i] = point;
			}

			return points;
		}
		public static Vector3 GetAroundTeleportationPosition(int unitIndex, int totalUnitCount, float radiusRandomOffset, float radius)
		{
			Vector3 diraction = Random.onUnitSphere;
			diraction.y = 0;
			if(diraction == Vector3.zero)
			{
				diraction = Vector3.forward;
			}

			return GetAroundMovePosition(unitIndex, totalUnitCount, radius, radiusRandomOffset, diraction);
		}
		public static Vector3[] GetAroundTeleportationPosition(int totalUnitCount, float radius, float radiusRandomOffset)
		{
			Vector3 diraction = Random.onUnitSphere;
			diraction.y = 0;
			if(diraction == Vector3.zero)
			{
				diraction = Vector3.forward;
			}

			return GetAroundMovePosition(totalUnitCount, radius, radiusRandomOffset, diraction);
		}
	}
}
