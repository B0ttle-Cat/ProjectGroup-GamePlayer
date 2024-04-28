using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class CreateFireunitObject : ComponentBehaviour
	{
		[SerializeField]
		public FireunitObject ObjectPrefab;

		private Dictionary<(int,int,int), CharacterObject> groupInUnit;

		private QuerySystem unitQuerySystem;
		private OdccQueryCollector unitQueryCollector;

		private QuerySystem characterQuerySystem;
		private OdccQueryCollector characterQueryCollector;

		public override void BaseAwake()
		{
			base.BaseAwake();
			if(ObjectPrefab != null)
			{
				ObjectPrefab.gameObject.SetActive(false);
			}

			groupInUnit = new Dictionary<(int, int, int), CharacterObject>();

			unitQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<FireunitObject, FireunitData>().Build();

			unitQueryCollector = OdccQueryCollector.CreateQueryCollector(unitQuerySystem)
				.CreateChangeListEvent(InitList, UpdateList)
				.CreateLooper(nameof(SyncUnitToCharacter), false)
				.Foreach<FireunitObject, IFireunitData>(SyncUnitToCharacter)
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
				unitQueryCollector.DeleteChangeListEvent(UpdateList);
				unitQueryCollector.DeleteLooper(nameof(SyncUnitToCharacter));
				unitQueryCollector = null;
			}
			if(characterQueryCollector != null)
			{
				characterQueryCollector.DeleteChangeListEvent(UpdateList);
				characterQueryCollector = null;
			}
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
				CreateObject(key);
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
				DestoryObject(key);
			}
		}

		private void CreateObject((int factionIndex, int teamIndex, int unitIndex) key)
		{
			if(ObjectPrefab == null) return;
			if(!ThisContainer.TryGetChildObject<FireunitObject>(out var _object, item => FindObject(item, key)))
			{
				ObjectPrefab.gameObject.SetActive(false);
				var createObject = GameObject.Instantiate(ObjectPrefab);
				createObject.ThisTransform.ResetLcoalPose(ThisTransform);

				if(!createObject.ThisContainer.TryGetData<FireunitData>(out var data))
				{
					data = createObject.ThisContainer.AddData<FireunitData>();
				}
				data.FactionIndex = key.factionIndex;
				data.TeamIndex = key.teamIndex;
				data.UnitIndex = key.unitIndex;
				createObject.UpdateObjectName();

				createObject.gameObject.SetActive(true);
			}
			static bool FindObject(FireunitObject _object, (int factionIndex, int teamIndex, int unitIndex) key)
			{
				return _object.ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsUnit(key.factionIndex, key.teamIndex, key.unitIndex);
			}
		}
		private void DestoryObject((int factionIndex, int teamIndex, int unitIndex) key)
		{
			if(ThisContainer.TryGetChildObject<FireunitObject>(out var _object, item => FindObject(item, key)))
			{
				Destroy(_object.gameObject);
			}

			static bool FindObject(FireunitObject _object, (int factionIndex, int teamIndex, int unitIndex) key)
			{
				return _object.ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsUnit(key.factionIndex, key.teamIndex, key.unitIndex);
			}
		}


		private void SyncUnitToCharacter(FireunitObject fireunitObject, IFireunitData iFireunitData)
		{
			if(!groupInUnit.TryGetValue((iFireunitData.FactionIndex, iFireunitData.TeamIndex, iFireunitData.UnitIndex), out var character))
			{
				return;
			}

			ModelObject modelObject = character.Model;
			if(modelObject != null)
			{
				var fireunitTransform = fireunitObject.ThisTransform;
				modelObject.ThisContainer.CallActionAllComponent<ICharacterAgent.TransformPose>(i =>
				{
					i.OnUpdatePose(fireunitTransform.position, fireunitTransform.rotation);
				});

				FireunitMovementAgent fireunitMovementAgent = fireunitObject.ThisContainer.GetComponent<FireunitMovementAgent>();
				if(fireunitMovementAgent != null && fireunitMovementAgent.NavMeshAgent != null)
				{
					float moveSpeed = fireunitMovementAgent.NavMeshAgent.velocity.magnitude;
					modelObject.ThisContainer.CallActionAllComponent<ICharacterAgent.MoveSpeed>(i =>
					{
						i.OnUpdateMoveSpeed(moveSpeed);
					});
				}
			}
		}
	}
}
