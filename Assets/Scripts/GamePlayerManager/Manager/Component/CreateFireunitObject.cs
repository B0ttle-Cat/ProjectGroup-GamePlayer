using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

using static BC.GamePlayerManager.StartGameSetting;

namespace BC.GamePlayerManager
{
	public class CreateFireunitObject : ComponentBehaviour
	{
		[SerializeField]
		public FireunitObject ObjectPrefab;

		private Dictionary<(int,int,int), CharacterObject> groupInUnit;

		private QuerySystem characterQuerySystem;
		[SerializeField] private OdccQueryCollector characterQueryCollector;

		public List<SpawnAnchor> SpawnList;

		public override void BaseAwake()
		{
			base.BaseAwake();
			if(ObjectPrefab != null)
			{
				ObjectPrefab.gameObject.SetActive(false);
			}

			groupInUnit = new Dictionary<(int, int, int), CharacterObject>();

			characterQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<CharacterObject, CharacterData>().Build();

			characterQueryCollector = OdccQueryCollector.CreateQueryCollector(characterQuerySystem)
				.CreateChangeListEvent(InitList, UpdateList);

		}
		public override void BaseDestroy()
		{
			base.BaseDestroy();

			if(characterQueryCollector != null)
			{
				characterQueryCollector.DeleteChangeListEvent(UpdateList);
				characterQueryCollector = null;
			}
			characterQuerySystem = null;

			groupInUnit = null;
			ObjectPrefab = null;
		}

		private void InitList(IEnumerable<ObjectBehaviour> initList)
		{
			foreach(var behaviour in initList)
			{
				if(behaviour is CharacterObject character)
				{
					if(character.ThisContainer.TryGetData<CharacterData>(out var data))
					{
						if(character.ThisContainer.TryGetData<SpawnData>(out var spawn))
						{
							AddedUnit(character, data, spawn);
						}
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
						if(character.ThisContainer.TryGetData<SpawnData>(out var spawn))
						{
							AddedUnit(character, data, spawn);
						}
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

		private void AddedUnit(CharacterObject character, CharacterData data, SpawnData spawn)
		{
			int factionIndex = data.FactionIndex;
			int teamIndex = data.TeamIndex;
			int unitIndex = data.UnitIndex;
			(int, int, int) key = (factionIndex, teamIndex, unitIndex);

			if(!groupInUnit.ContainsKey(key))
			{
				groupInUnit.Add(key, character);
				CreateObject(key, spawn);
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

		private void CreateObject((int factionIndex, int teamIndex, int unitIndex) key, SpawnData spawn)
		{
			if(ObjectPrefab == null) return;
			if(!ThisContainer.TryGetChildObject<FireunitObject>(out var _object, item => FindObject(item, key)))
			{
				ObjectPrefab.gameObject.SetActive(false);
				var unitObject = GameObject.Instantiate(ObjectPrefab);
				unitObject.ThisTransform.ResetLcoalPose(ThisTransform);

				if(!unitObject.ThisContainer.TryGetData<FireunitData>(out var data))
				{
					data = unitObject.ThisContainer.AddData<FireunitData>();
				}
				data.FactionIndex = key.factionIndex;
				data.TeamIndex = key.teamIndex;
				data.UnitIndex = key.unitIndex;
				unitObject.UpdateObjectName();

				unitObject.ThisContainer.RemoveData<SpawnData>();
				unitObject.ThisContainer.RemoveComponent<SpawnComponent>();

				unitObject.ThisContainer.AddData<SpawnData>(spawn);
				unitObject.ThisContainer.AddComponent<SpawnComponent>();

				unitObject.gameObject.SetActive(true);
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
	}
}
