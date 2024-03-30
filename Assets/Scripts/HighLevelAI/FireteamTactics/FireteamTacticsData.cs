using System;

using BC.ODCC;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FireteamTacticsData : DataObject
	{
		[Header("Variable")]
		public Variable variable;

		[Header("Data")]
		public Vector3 tacticsPosition;

		// public StrategicPath TargetPath;
		// public List<StrategicPath> fullPathList;

		//[Header("Result")]
		//public StrategicPath movePath;
		//public StrategicPoint movePoint;
		//public int targetCost;


		[Serializable]
		public class Variable
		{
			[Range(0f,10f)]
			public float stayVariable = 1f;
			[Range(0f,10f)]
			public float moveVariable = 1f;
		}
	}

}
