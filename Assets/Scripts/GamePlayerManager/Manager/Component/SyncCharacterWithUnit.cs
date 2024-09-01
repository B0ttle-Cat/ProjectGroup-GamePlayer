using System.Collections.Generic;

using BC.Character;
using BC.LowLevelAI;
using BC.ODCC;
using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayerManager
{
	public class SyncCharacterWithUnit : ComponentBehaviour
	{
		private Dictionary<int, CharacterObject> groupInUnit;

		private QuerySystem characterQuerySystem;
		[SerializeField]
		private OdccQueryCollector characterQueryCollector;

		private QuerySystem unitQuerySystem;
		[SerializeField]
		private OdccQueryCollector unitQueryCollector;


		public override void BaseAwake()
		{
			base.BaseAwake();

			groupInUnit = new Dictionary<int, CharacterObject>();

			unitQuerySystem = QuerySystemBuilder.CreateQuery()
			.WithAll<FireunitObject, FireunitData, IUnitInteractiveValue>().Build();

			unitQueryCollector = OdccQueryCollector.CreateQueryCollector(unitQuerySystem)
				.CreateLooperEvent(nameof(SyncCharacterWithUnit), 1)
				.Foreach<FireunitObject, FireunitData, IUnitInteractiveValue>(OnSyncUnitToCharacter)
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
			int key = data.MemberUniqueID;

			if(!groupInUnit.ContainsKey(key))
			{
				groupInUnit.Add(key, character);
			}
		}
		private void RemoveUnit(CharacterObject character, CharacterData data)
		{
			int key = data.MemberUniqueID;

			if(groupInUnit.ContainsKey(key))
			{
				groupInUnit.Remove(key);
			}
		}
		private void OnSyncUnitToCharacter(OdccQueryLooper.LoopInfo loopInfo, FireunitObject fireunitObject, FireunitData fireunitData, IUnitInteractiveValue unitInteractiveValue)
		{
			int thisUnitKey = fireunitData.MemberUniqueID;

			if(!groupInUnit.TryGetValue(thisUnitKey, out var character))
			{
				return;
			}
			if(!character.ThisContainer.TryGetComponent<ICharacterAgent>(out var characterAgent))
			{
				return;
			}

			SyncTransformPose();
			void SyncTransformPose()
			{
				Transform unitTransform = fireunitObject.ThisTransform;
				if(unitTransform.hasChanged)
				{
					characterAgent.TransformPose.OnUpdatePose(unitTransform.position, unitTransform.rotation);
					unitTransform.hasChanged = false;
				}
			}

			WalkAnimation();
			void WalkAnimation()
			{
				IUnitIMovementAgent movementAgent = fireunitObject.ThisContainer.GetComponent<IUnitIMovementAgent>();
				ITacticalCombatStateValue.TacticalCombatStateType tacticalCombatState = unitInteractiveValue.StateValueData.TacticalCombatState;
				if(movementAgent != null && movementAgent.NavMeshAgent != null)
				{
					float moveSpeed = movementAgent.NavMeshAgent.velocity.magnitude;
					bool aiming = movementAgent.NavMeshAgent.isStopped && tacticalCombatState == ITacticalCombatStateValue.TacticalCombatStateType.Attack;
					characterAgent.Animation.OnUpdateMoveSpeed(moveSpeed);
				}
			}

			AttackAnimation();
			void AttackAnimation()
			{
				IUnitAttackerAgent attackerAgent = fireunitObject.ThisContainer.GetComponent<IUnitAttackerAgent>();
				IUnitInteractiveValue targetValue = attackerAgent.ThisTarget;
				if(targetValue == null || !groupInUnit.TryGetValue(targetValue.MemberUniqueID, out var targetObj))
				{
					return;
				}
				if(!targetObj.ThisContainer.TryGetComponent<ICharacterAgent>(out var targetAgent))
				{
					return;
				}

				ICharacterAgent.IWeapon characterWeapon = characterAgent.Weapon;
				characterWeapon.OnShotAttack(targetAgent, Shot);

				void Shot(ICharacterAgent actorAgent, ICharacterAgent targetAgent)
				{
					(ICharacterAgent agent, IUnitInteractiveValue value) actor = new (actorAgent, attackerAgent.ThisActor);
					(ICharacterAgent agent, IUnitInteractiveValue value) target = new (targetAgent, attackerAgent.ThisTarget);
					characterWeapon.OnCreateProjectile(actor, target, Hit);
				}
				void Hit(IProjectileObject projectileObject, ProjectileHitReport hitReport)
				{

				}
			}
		}
	}
}
