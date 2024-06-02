using System.Collections.Generic;

using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

namespace BC.GamePlayerManager
{
	public class SyncCharacterWithUnit : ComponentBehaviour
	{
		private Dictionary<(int,int,int), CharacterObject> groupInUnit;

		private QuerySystem characterQuerySystem;
		private OdccQueryCollector characterQueryCollector;

		private QuerySystem unitQuerySystem;
		private OdccQueryCollector unitQueryCollector;


		public override void BaseAwake()
		{
			base.BaseAwake();

			groupInUnit = new Dictionary<(int, int, int), CharacterObject>();

			unitQuerySystem = QuerySystemBuilder.CreateQuery()
			.WithAll<FireunitObject, FireunitData>().Build();

			unitQueryCollector = OdccQueryCollector.CreateQueryCollector(unitQuerySystem)
				.CreateLooperEvent(nameof(SyncCharacterWithUnit), false)
				.Foreach<FireunitObject, IFireunitData>(OnSyncUnitToCharacter)
				.GetCollector();

			characterQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<CharacterObject, CharacterData>().Build();

			characterQueryCollector = OdccQueryCollector.CreateQueryCollector(characterQuerySystem)
				.CreateChangeListEvent(InitList, UpdateList);
		}

		public override void BaseDestroy()
		{
			base.BaseDestroy();

			if(unitQueryCollector != null)
			{
				unitQueryCollector.DeleteLooperEvent(nameof(OnSyncUnitToCharacter));
				unitQueryCollector = null;
			}
			unitQuerySystem = null;

			if(characterQueryCollector != null)
			{
				characterQueryCollector.DeleteChangeListEvent(UpdateList);
				characterQueryCollector = null;
			}
			characterQuerySystem = null;

			groupInUnit = null;
		}
		private void InitList(IEnumerable<ObjectBehaviour> initList)
		{
			foreach(var behaviour in initList)
			{
				if(behaviour is CharacterObject character)
				{
					if(character.ThisContainer.TryGetData<CharacterData>(out var data))
					{
						AddedUnit(character, data);
					}
				}
			}
		}
		private void UpdateList(ObjectBehaviour behaviour, bool added)
		{
			if(added)
			{
				if(behaviour is CharacterObject character)
				{
					if(character.ThisContainer.TryGetData<CharacterData>(out var data))
					{
						AddedUnit(character, data);
					}
				}
			}
			else
			{
				if(behaviour is CharacterObject character)
				{
					if(character.ThisContainer.TryGetData<CharacterData>(out var data))
					{
						RemoveUnit(character, data);
					}
				}
			}
		}
		private void AddedUnit(CharacterObject character, CharacterData data)
		{
			int factionIndex = data.FactionIndex;
			int teamIndex = data.TeamIndex;
			int unitIndex = data.UnitIndex;
			(int, int, int) key = (factionIndex, teamIndex, unitIndex);

			if(!groupInUnit.ContainsKey(key))
			{
				groupInUnit.Add(key, character);
			}
		}
		private void RemoveUnit(CharacterObject character, CharacterData data)
		{
			int factionIndex = data.FactionIndex;
			int teamIndex = data.TeamIndex;
			int unitIndex = data.UnitIndex;
			(int,int, int) key = (factionIndex, teamIndex, unitIndex);

			if(groupInUnit.ContainsKey(key))
			{
				groupInUnit.Remove(key);
			}
		}
		private void OnSyncUnitToCharacter(OdccQueryLooper.LoopInfo loopInfo, FireunitObject fireunitObject, IFireunitData iFireunitData)
		{
			if(!groupInUnit.TryGetValue((iFireunitData.FactionIndex, iFireunitData.TeamIndex, iFireunitData.UnitIndex), out var character))
			{
				return;
			}

			ModelObject modelObject = character.Model;
			if(modelObject != null)
			{
				var fireunitTransform = fireunitObject.ThisTransform;
				modelObject.ThisContainer.CallActionAllComponent<ICharacterAgent.TransformPose>(i => {
					if(fireunitTransform.hasChanged)
					{
						i.OnUpdatePose(fireunitTransform.position, fireunitTransform.rotation);
						fireunitTransform.hasChanged = false;
					}
				});

				FireunitMovementAgent fireunitMovementAgent = fireunitObject.ThisContainer.GetComponent<FireunitMovementAgent>();
				if(fireunitMovementAgent != null && fireunitMovementAgent.NavMeshAgent != null)
				{
					float moveSpeed = fireunitMovementAgent.NavMeshAgent.velocity.magnitude;
					modelObject.ThisContainer.CallActionAllComponent<ICharacterAgent.MoveSpeed>(i => {
						i.OnUpdateMoveSpeed(moveSpeed);
					});
				}
			}
		}
	}
}
