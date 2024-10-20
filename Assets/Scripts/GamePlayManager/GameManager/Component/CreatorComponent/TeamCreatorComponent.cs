using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayManager
{
	public class TeamCreatorComponent : ComponentBehaviour, ITeamCreatorComponent
	{
		[SerializeField]
		private FireteamObject ObjectPrefab;

		private Dictionary<(int,int), List<CharacterData>> groupInUnit;

		private QuerySystem characterQuerySystem;
		[SerializeField] private OdccQueryCollector characterQueryCollector;

		public override void BaseAwake()
		{
			base.BaseAwake();
			if(ObjectPrefab != null)
			{
				ObjectPrefab.gameObject.SetActive(false);
			}

			groupInUnit = new Dictionary<(int, int), List<CharacterData>>();

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
						AddedUnit(data);
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
						AddedUnit(data);
					}
				}
			}
			else
			{
				if(behaviour is CharacterObject character)
				{
					if(character.ThisContainer.TryGetData<CharacterData>(out var data))
					{
						RemoveUnit(data);
					}
				}
			}
		}

		private void AddedUnit(CharacterData data)
		{
			int factionIndex = data.FactionIndex;
			int teamIndex = data.TeamIndex;
			(int,int) key = (factionIndex, teamIndex);

			if(!groupInUnit.ContainsKey(key))
			{
				groupInUnit.Add(key, new List<CharacterData>());
				CreateObject(key);
			}
			int findIndex = groupInUnit[key].FindIndex(i => i.IsEqualsUnit(data));
			if(findIndex < 0)
			{
				groupInUnit[key].Add(data);
			}
		}
		private void RemoveUnit(CharacterData data)
		{
			int factionIndex = data.FactionIndex;
			int teamIndex = data.TeamIndex;
			(int,int) key = (factionIndex, teamIndex);

			if(!groupInUnit.ContainsKey(key))
			{
				return;
			}
			int findIndex = groupInUnit[key].FindIndex(i => i.IsEqualsUnit(data));
			if(findIndex >= 0)
			{
				groupInUnit[key].RemoveAt(findIndex);

				if(groupInUnit.Count == 0)
				{
					groupInUnit.Remove(key);
					DestoryObject(key);
				}
			}
		}


		private void CreateObject((int factionIndex, int teamIndex) key)
		{
			if(ObjectPrefab == null) return;
			if(!ThisContainer.TryGetChildObject<FireteamObject>(out var _object, item => FindObject(item, key)))
			{
				ObjectPrefab.gameObject.SetActive(false);
				var createObject = GameObject.Instantiate(ObjectPrefab);
				createObject.ThisTransform.ResetLcoalPose(ThisTransform);

				if(!createObject.ThisContainer.TryGetData<FireteamData>(out var data))
				{
					data = createObject.ThisContainer.AddData<FireteamData>();
				}

				data.FactionIndex = key.factionIndex;
				data.TeamIndex = key.teamIndex;
				createObject.UpdateObjectName();

				createObject.gameObject.SetActive(true);
			}
			static bool FindObject(FireteamObject _object, (int factionIndex, int teamIndex) key)
			{
				return _object.ThisContainer.TryGetData<IFireteamData>(out var data) && data.IsEqualsTeam(key.factionIndex, key.teamIndex);
			}
		}
		private void DestoryObject((int factionIndex, int teamIndex) key)
		{
			if(ThisContainer.TryGetChildObject<FireteamObject>(out var _object, item => FindObject(item, key)))
			{
				Destroy(_object.gameObject);
			}

			static bool FindObject(FireteamObject _object, (int factionIndex, int teamIndex) key)
			{
				return _object.ThisContainer.TryGetData<IFireteamData>(out var data) && data.IsEqualsTeam(key.factionIndex, key.teamIndex);
			}
		}
	}
}
