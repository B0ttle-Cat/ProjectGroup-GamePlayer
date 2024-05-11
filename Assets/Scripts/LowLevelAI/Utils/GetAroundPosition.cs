using UnityEngine;

namespace BC.LowLevelAI
{
	public static class GetAroundPosition
	{
		public static Vector3 GetAroundMovePosition(int unitIndex, int positionCount, float radius, Vector3 moveDiraction)
		{
			if(positionCount == 0 || positionCount == 1)
			{
				return Vector3.zero;
			}
			moveDiraction = moveDiraction.normalized;

			Vector3 randomPos = Random.onUnitSphere * (radius / (positionCount + 1));
			float splitAngle = 360f / positionCount;
			float angle = unitIndex * splitAngle;
			Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * moveDiraction;
			return randomPos + direction * radius;
		}
		public static Vector3[] GetAroundMovePosition(int positionCount, float radius, Vector3 moveDiraction)
		{
			if(positionCount == 0 || positionCount == 1)
			{
				return new Vector3[1] { Vector3.zero };
			}
			moveDiraction = moveDiraction.normalized;

			Vector3[] points = new Vector3[positionCount];
			Vector3 randomPos = Random.onUnitSphere * (radius / (positionCount + 1));
			float splitAngle = 360f / positionCount;

			for(int i = 0 ; i < positionCount ; i++)
			{
				float angle = i * splitAngle;
				Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * moveDiraction;
				Vector3 point = randomPos + direction * radius;

				points[i] = point;
			}

			return points;
		}
		public static Vector3 GetAroundSpwanPosition(int unitIndex, int totalUnitCount, float radius)
		{
			Vector3 diraction = Random.onUnitSphere;
			diraction.y = 0;
			if(diraction == Vector3.zero)
			{
				diraction = Vector3.forward;
			}

			return GetAroundMovePosition(unitIndex, totalUnitCount, radius, diraction);
		}
		public static Vector3[] GetAroundSpwanPosition(int totalUnitCount, float radius)
		{
			Vector3 diraction = Random.onUnitSphere;
			diraction.y = 0;
			if(diraction == Vector3.zero)
			{
				diraction = Vector3.forward;
			}

			return GetAroundMovePosition(totalUnitCount, radius, diraction);
		}
	}
}
