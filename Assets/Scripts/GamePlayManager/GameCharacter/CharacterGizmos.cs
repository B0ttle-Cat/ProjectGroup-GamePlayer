using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class CharacterGizmos : ComponentBehaviour
	{
		[SerializeField]
		private Renderer factionGizmosRenderer;
		private Material factionGizmosMaterial;
		private IFireunitData fireunitData;

		public override async void BaseEnable()
		{
			if(factionGizmosRenderer == null) return;
			factionGizmosRenderer.gameObject.SetActive(false);
			factionGizmosMaterial = factionGizmosRenderer.material;
			if(factionGizmosMaterial == null) return;

			fireunitData = await ThisContainer.AwaitGetData<IFireunitData>();

			if(OdccManager.TryFindOdccObject<GamePlayManager>(QuerySystemBuilder.SimpleQueryBuild<GamePlayManager>(), out var findObj))
			{
				StartLevelData startLevelData = findObj.ThisContainer.GetData<StartLevelData>();
				var factionSetting = startLevelData.FactionSetting;
				if(factionSetting == null) return;

				int findIndex = factionSetting.factionSettingList.FindIndex(i => fireunitData.IsEqualsFaction(i));
				if(findIndex < 0) return;

				var findFaction = factionSetting.factionSettingList[findIndex];
				Color factionColor = findFaction.FactionColor;

				factionGizmosMaterial.color = factionColor;
			}

			factionGizmosRenderer.material = factionGizmosMaterial;
			factionGizmosRenderer.gameObject.SetActive(true);
		}
	}
}
