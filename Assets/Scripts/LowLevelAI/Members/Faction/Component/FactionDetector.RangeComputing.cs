using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class FactionDetector// RangeComputing
	{
		internal bool RangeComputing(FireunitDetector myDetector, FireunitDetector targetDetector)
		{
			Vector3 detectorPosition = myDetector.DetectorPositionXZ;
			Vector3 lookForward = myDetector.LookForwardXZ;

			Vector3 targetPosition = targetDetector.DetectorPositionXZ;
			float aidDetectorRange = myDetector.AidDetectorRange;
			float antiDetectorRange = targetDetector.AntiDetectorRange;
			float chageDetectorValue = aidDetectorRange - antiDetectorRange;
			DetectorRangeData detectorRangeDatas = myDetector.DetectorRangeData;

			if(detectorRangeDatas == null) return false;

			float baseRadius = myDetector.ColliderRadius + targetDetector.ColliderRadius;

			Vector3 targetDir = targetPosition - detectorPosition;
			float requiredDistance = targetDir.magnitude;
			Vector3 targetNormal = targetDir.normalized;

			bool isDetected = false;
			if(Calculation(detectorRangeDatas, requiredDistance, targetNormal, lookForward, baseRadius, chageDetectorValue))
			{
				isDetected = true;
			}
			return isDetected;
		}

		private bool Calculation(DetectorRangeData rangesConfig,
			float requiredDistance, Vector3 targetNormal, Vector3 lookForward,
			float baseRadius, float chageDetectorValue)
		{
			float detectingRnage = rangesConfig.detectingRnage + baseRadius;
			float minimumDetectingRnage = rangesConfig.minimumDetectingRnage + baseRadius;
			float maximumDetectingRnage = rangesConfig.maximumDetectingRnage + baseRadius;
			float detectingYAngle = rangesConfig.detectingYAngle;

			detectingRnage += chageDetectorValue;

			if(detectingRnage < minimumDetectingRnage)
				detectingRnage = minimumDetectingRnage;
			if(detectingRnage > maximumDetectingRnage)
				detectingRnage = maximumDetectingRnage;

			bool isDetedted = requiredDistance <= detectingRnage;

			if(isDetedted && detectingYAngle < 179.99f)
			{
				Vector3 lookDir = Quaternion.Euler(0f, detectingYAngle, 0f) * lookForward;
				float angle =  Vector3.Angle(lookDir, targetNormal);

				if(angle <= detectingYAngle)
				{
					isDetedted  = true;
				}
			}
			return isDetedted;
		}
	}
}
