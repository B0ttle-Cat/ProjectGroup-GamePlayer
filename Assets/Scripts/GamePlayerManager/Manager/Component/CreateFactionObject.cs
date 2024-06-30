using System.Collections.Generic;

using BC.Base;
using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class CreateFactionObject : ComponentBehaviour
	{
		[SerializeField]
		private FactionObject ObjectPrefab;

		private Dictionary<int, List<CharacterData>> groupInUnit;

		private QuerySystem characterQuerySystem;
		[SerializeField] private OdccQueryCollector characterQueryCollector;

		public override void BaseAwake()
		{
			base.BaseAwake();
			if(ObjectPrefab != null)
			{
				ObjectPrefab.gameObject.SetActive(false);
			}

			groupInUnit = new Dictionary<int, List<CharacterData>>();

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
			int index = data.FactionIndex;
			if(!groupInUnit.ContainsKey(index))
			{
				groupInUnit.Add(index, new List<CharacterData>());
				CreateObject(index);
			}
			int findIndex = groupInUnit[index].FindIndex(i => i.IsEqualsUnit(data));
			if(findIndex < 0)
			{
				groupInUnit[index].Add(data);
			}
		}
		private void RemoveUnit(CharacterData data)
		{
			int index = data.FactionIndex;
			if(!groupInUnit.ContainsKey(index))
			{
				return;
			}
			int findIndex = groupInUnit[index].FindIndex(i => i.IsEqualsUnit(data));
			if(findIndex >= 0)
			{
				groupInUnit[index].RemoveAt(findIndex);

				if(groupInUnit.Count == 0)
				{
					groupInUnit.Remove(index);
					DestoryObject(index);
				}
			}
		}


		private void CreateObject(int factionIndex)
		{
			if(ObjectPrefab == null) return;

			if(!ThisContainer.TryGetChildObject<FactionObject>(out var faction, item => FindObject(item, factionIndex)))
			{
				ObjectPrefab.gameObject.SetActive(false);
				var createObject = GameObject.Instantiate(ObjectPrefab);
				createObject.ThisTransform.ResetLcoalPose(ThisTransform);

				if(!createObject.ThisContainer.TryGetData<FactionData>(out var data))
				{
					data = createObject.ThisContainer.AddData<FactionData>();
				}
				data.FactionIndex = factionIndex;
				createObject.UpdateObjectName();

				if(ThisContainer.TryGetData<DiplomacyData>(out var diplomacy))
				{
					if(diplomacy.GetDiplomacy(factionIndex, out var diplomacyItem))
					{
						if(diplomacyItem.IsAIFaction)
						{
							createObject.gameObject.AddComponent<AIGamePlayerInterface>();
						}
						else
						{
							createObject.gameObject.AddComponent<HumanPlayerInterface>();
						}
					}
				}
				createObject.gameObject.SetActive(true);
			}
			static bool FindObject(FactionObject factionObject, int factionIndex)
			{
				return factionObject.ThisContainer.TryGetData<IFactionData>(out var data) && data.IsEqualsFaction(factionIndex);
			}
		}
		private void DestoryObject(int factionIndex)
		{
			if(ThisContainer.TryGetChildObject<FactionObject>(out var faction, item => FindObject(item, factionIndex)))
			{
				Destroy(faction.gameObject);
			}

			static bool FindObject(FactionObject factionObject, int factionIndex)
			{
				return factionObject.ThisContainer.TryGetData<IFactionData>(out var data) && data.IsEqualsFaction(factionIndex);
			}
		}
	}
}
