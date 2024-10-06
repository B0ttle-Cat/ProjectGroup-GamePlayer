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

		private Dictionary<Vector3Int, CharacterObject> groupInUnit;

		private QuerySystem characterQuerySystem;
		[SerializeField] private OdccQueryCollector characterQueryCollector;

		public List<TeamSettingInfo> TeamSettingList;

		public override void BaseAwake()
		{
			base.BaseAwake();
			if(ObjectPrefab != null)
			{
				ObjectPrefab.gameObject.SetActive(false);
			}

			groupInUnit = new Dictionary<Vector3Int, CharacterObject>();

			characterQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<CharacterObject, CharacterData, CreateSettingInfo>().Build();

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
					if(character.ThisContainer.TryGetData<CreateSettingInfo>(out var data))
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
					if(character.ThisContainer.TryGetData<CreateSettingInfo>(out var data))
					{
						AddedUnit(character, data);
					}
				}
			}
			else
			{
				if(behaviour is CharacterObject character)
				{
					if(character.ThisContainer.TryGetData<CreateSettingInfo>(out var data))
					{
						RemoveUnit(character, data);
					}
				}
			}
		}

		private void AddedUnit(CharacterObject character, CreateSettingInfo data)
		{
			int factionIndex = data.FactionIndex;
			int teamIndex = data.TeamIndex;
			int unitIndex = data.UnitIndex;
			Vector3Int key = data.MemberUniqueID;

			if(!groupInUnit.ContainsKey(key))
			{
				groupInUnit.Add(key, character);
				CreateObject(data);
			}
		}
		private void RemoveUnit(CharacterObject character, CreateSettingInfo data)
		{
			Vector3Int key = data.MemberUniqueID;

			if(groupInUnit.ContainsKey(key))
			{
				groupInUnit.Remove(key);
				DestoryObject(key);
			}
		}

		private void CreateObject(CreateSettingInfo createSetting)
		{
			if(ObjectPrefab == null) return;
			if(createSetting == null) return;
			Vector3Int key = createSetting.MemberUniqueID;
			if(!ThisContainer.TryGetChildObject<FireunitObject>(out var _object, item => FindObject(item, key)))
			{
				ObjectPrefab.gameObject.SetActive(false);
				var unitObject = GameObject.Instantiate(ObjectPrefab);
				unitObject.ThisTransform.ResetLcoalPose(ThisTransform);

				if(!unitObject.ThisContainer.TryGetData<FireunitData>(out var data))
				{
					data = unitObject.ThisContainer.AddData<FireunitData>();
				}
				data.FactionIndex = createSetting.FactionIndex;
				data.TeamIndex = createSetting.TeamIndex;
				data.UnitIndex = createSetting.UnitIndex;
				unitObject.UpdateObjectName();

				if(!unitObject.ThisContainer.TryGetComponent<UnitInitSetter>(out var unitInitSetter))
				{
					unitInitSetter = unitObject.ThisContainer.AddComponent<UnitInitSetter>();
				}
				unitInitSetter.fireunitData = data;
				unitInitSetter.factionSettingInfo = createSetting.factionSettingInfo;
				unitInitSetter.teamSettingInfo = createSetting.teamSettingInfo;
				unitInitSetter.unitSettingInfo = createSetting.unitSettingInfo;
				unitInitSetter.characterSettingInfo = createSetting.characterSettingInfo;
				createSetting.EndSetupFireunit();

				unitObject.gameObject.SetActive(true);
			}
			static bool FindObject(FireunitObject _object, Vector3Int key)
			{
				return _object.ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsUnit(key);
			}
		}
		private void DestoryObject(Vector3Int key)
		{
			if(ThisContainer.TryGetChildObject<FireunitObject>(out var _object, item => FindObject(item, key)))
			{
				Destroy(_object.gameObject);
			}

			static bool FindObject(FireunitObject _object, Vector3Int key)
			{
				return _object.ThisContainer.TryGetData<IFireunitData>(out var data) && data.IsEqualsUnit(key);
			}
		}
	}
}
