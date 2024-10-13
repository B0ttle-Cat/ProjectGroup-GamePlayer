using BC.ODCC;

namespace BC.LowLevelAI
{
	public class FireteamSpawnOnAnchorActor : FireteamCommandActor<FireteamSpawnOnAnchorData>, IOdccUpdate.Late
	{
		public override void BaseActorEnable()
		{
			StartSpawn();
		}
		public override void BaseActorDisable()
		{
			StopSpawn();
		}
		public override void BaseActorLateUpdate()
		{
			if(CheckIsSpawnEnd())
			{
				Destroy(this);
			}
		}



		private void StartSpawn()
		{
			int index = 0;
			int total = FireteamMembers.Count;
			FireteamMembers.Foreach(unitObject => {
				var spawn = new SpawnData() {
					targetAnchor = CommandData.SpawnAnchor,
					totalUnitCount = total,
					unitIndex = index++,
					targetRadius = 2f,
				};
				unitObject.ThisContainer.RemoveData<SpawnData>();
				unitObject.ThisContainer.RemoveComponent<FireunitSpawnActor>();

				unitObject.ThisContainer.AddData<SpawnData>(spawn);
				unitObject.ThisContainer.AddComponent<FireunitSpawnActor>();
			});
		}
		private void StopSpawn()
		{
			FireteamMembers.Foreach(unitObject => {
				unitObject.ThisContainer.RemoveData<SpawnData>();
				unitObject.ThisContainer.RemoveComponent<FireunitSpawnActor>();
			});
		}
		private bool CheckIsSpawnEnd()
		{
			if(FireteamMembers.HasCondition(member => member.TryGetComponent<FireunitSpawnActor>(out _)))
			{
				return false;
			}
			return true;
		}

		public override void BaseLateUpdate()
		{
			base.BaseLateUpdate();
		}
	}
}
