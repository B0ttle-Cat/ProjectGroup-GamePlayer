using System;
using System.Collections.Generic;

using BC.LowLevelAI;
using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.HighLevelAI
{
	public class FactionStrategyData : DataObject
	{
		[Header("Variable")]
		public Variable variable;

		[Header("Data")]
		public List<StrategicPath> NextStrategicPath;
		public List<float> totalCostOfPath;

		[Header("Result")]
		public int ResultCount = 1;
		[ReadOnly]
		public List<StrategicPath> selectcStrategicPath;


		[Serializable]
		public class Variable
		{
			[Range(0f,10f)]
			public float costVariable = 1f;
			[Range(0f,10f)]
			public float pointVariable = 1f;
		}
	}

}
