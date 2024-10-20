using BC.ODCC;
using BC.OdccBase;

namespace BC.GamePlayManager
{
	public class GameUnitSetter : ComponentBehaviour, IStartSetup
	{
		public bool isCompleteSetting;
		public bool IsCompleteSetting {
			get {
				if(isCompleteSetting) return true;
				//isCompleteSetting = ;
				return isCompleteSetting;
			}
			set {
				isCompleteSetting = value;
			}
		}


		StartLevelData startLevelData;
		public override void BaseAwake()
		{
			base.BaseAwake();
			ThisContainer.TryGetData<StartLevelData>(out startLevelData);
		}

		public override void BaseDestroy()
		{
			base.BaseDestroy();

			startLevelData = null;
		}

		public override void BaseEnable()
		{
			isCompleteSetting = false;
		}

		public override void BaseDisable()
		{
			//startLevelData.UnitSetting
		}
	}
}
