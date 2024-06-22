using System.Collections.Generic;

using BC.ODCC;

namespace BC.OdccBase
{
	public interface IDetectorUpdate : IOdccComponent
	{
		public bool StartCompute();
		public void EndedCompute();

		public List<FireunitData> DetectingThisFrameList { get; set; }
	}
}
