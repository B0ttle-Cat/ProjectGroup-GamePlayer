using BC.HighLevelAI;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class GamePlayManager : ObjectBehaviour, IGamePlayManager
	{
		[SerializeField, InlineProperty, HideInPlayMode, HideLabel]
		private StartLevelData startLevelData;

		[SerializeField] private LowLevelAIManager lowLevelAIManager;
		[SerializeField] private HighLevelAIManager highLevelAIManager;
		private MapCreatorComponent mapCreatorComponent;
		private CharacterCreatorComponent characterCreatorComponent;
		private FactionCreatorComponent factionCreatorComponent;
		private TeamCreatorComponent teamCreatorComponent;
		private UnitCreatorComponent unitCreatorComponent;

		ILowLevelAIManager IGamePlayManager.LowLevelAI => lowLevelAIManager;
		IHighLevelAIManager IGamePlayManager.HighLevelAI => highLevelAIManager;
		IMapCreatorComponent IGamePlayManager.MapCreatorComponent => mapCreatorComponent;
		ICharacterCreatorComponent IGamePlayManager.CharacterCreatorComponent => characterCreatorComponent;
		IFactionCreatorComponent IGamePlayManager.FactionCreatorComponent => factionCreatorComponent;
		ITeamCreatorComponent IGamePlayManager.TeamCreatorComponent => teamCreatorComponent;
		IUnitCreatorComponent IGamePlayManager.UnitCreatorComponent => unitCreatorComponent;


		public override void BaseValidate()
		{
			base.BaseValidate();

			lowLevelAIManager = lowLevelAIManager != null ? lowLevelAIManager : FindAnyObjectByType<LowLevelAIManager>();
			highLevelAIManager = highLevelAIManager != null ? highLevelAIManager : FindAnyObjectByType<HighLevelAIManager>();
		}
		public override void BaseAwake()
		{
			base.BaseAwake();

			ThisContainer.AddData<StartLevelData>(startLevelData);

			lowLevelAIManager = lowLevelAIManager != null ? lowLevelAIManager : FindAnyObjectByType<LowLevelAIManager>();
			highLevelAIManager = highLevelAIManager != null ? highLevelAIManager : FindAnyObjectByType<HighLevelAIManager>();

			mapCreatorComponent = ThisContainer.GetComponentInChild<MapCreatorComponent>();
			characterCreatorComponent = ThisContainer.GetComponentInChild<CharacterCreatorComponent>();
			factionCreatorComponent = ThisContainer.GetComponentInChild<FactionCreatorComponent>();
			teamCreatorComponent = ThisContainer.GetComponentInChild<TeamCreatorComponent>();
			unitCreatorComponent = ThisContainer.GetComponentInChild<UnitCreatorComponent>();
		}
		public override void BaseDestroy()
		{
			lowLevelAIManager  = null;
			highLevelAIManager = null;
		}



		public override void BaseEnable()
		{
			if(ThisContainer.TryGetComponent<GameLevelSetter>(out var levelSetter))
			{
				if(levelSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = true;
				}
			}

			if(ThisContainer.TryGetComponent<GamePlayerSetter>(out var playerSetter))
			{
				if(playerSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = true;
				}
			}

			if(ThisContainer.TryGetComponent<GameUnitSetter>(out var unitSetter))
			{
				if(unitSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = true;
				}
			}
		}

		public override void BaseDisable()
		{
			if(ThisContainer.TryGetComponent<GameLevelSetter>(out var levelSetter))
			{
				if(levelSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = false;
				}
			}

			if(ThisContainer.TryGetComponent<GamePlayerSetter>(out var playerSetter))
			{
				if(playerSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = false;
				}
			}

			if(ThisContainer.TryGetComponent<GameUnitSetter>(out var unitSetter))
			{
				if(unitSetter is IStartSetup iSetter)
				{
					iSetter.ThisMono.enabled = false;
				}
			}
		}
	}
}
