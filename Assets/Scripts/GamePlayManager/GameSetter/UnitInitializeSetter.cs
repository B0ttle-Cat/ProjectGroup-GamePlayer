using BC.LowLevelAI;
using BC.Map;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class UnitInitializeSetter : ComponentBehaviour
	{
		public IFireunitData fireunitData;

		public IGamePlayManager gamePlayManager;
		public FactionSettingInfo factionSettingInfo;
		public TeamSettingInfo teamSettingInfo;
		public UnitSettingInfo unitSettingInfo;
		public CharacterSettingInfo characterSettingInfo;

		public override async void BaseStart()
		{
			while(gamePlayManager == null)
			{
				await Awaitable.NextFrameAsync();
			}

			// 기본 변수 세팅
			var fireunitInteractiveValue = await ThisContainer.AwaitGetComponent<FireunitInteractiveValue>();
			fireunitInteractiveValue.TypeValueData.Paste(characterSettingInfo.TypeSetter);
			fireunitInteractiveValue.PlayValueData.Paste(characterSettingInfo.ValueSetter);

			var map = gamePlayManager.MapCreatorComponent;

			MapCreatorComponent createMapObject = await gamePlayManager.ThisContainer.AwaitGetComponent<MapCreatorComponent>(i=> i.MapAnchor != null && i.MapAnchor.IsCompleteUpdate);
			MapPathPointComputer mapComputer = createMapObject.MapAnchor;
			if(mapComputer.TrySelectAnchorIndex(teamSettingInfo.AnchorIndex, out var mapAnchor))
			{
				// 스폰 위치 세팅
				var agentMove = await ThisContainer.AwaitGetComponent<IUnitIMovementAgent>();
				Vector3 anchorPosition = mapAnchor.ThisPosition() + Random.insideUnitSphere * 1f;
				Vector3 randomOffset = Random.insideUnitSphere * 1.5f;

				agentMove.InputMoveTarget(anchorPosition, true);
				agentMove.InputMoveTarget(anchorPosition + randomOffset);
			}


			DestroyThis();
		}
	}
}
