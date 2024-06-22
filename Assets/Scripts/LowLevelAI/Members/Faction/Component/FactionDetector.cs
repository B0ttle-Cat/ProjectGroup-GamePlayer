using System.Collections.Generic;
using System.Linq;

using BC.ODCC;
using BC.OdccBase;

using Sirenix.OdinInspector;

using UnityEngine;

namespace BC.LowLevelAI
{
	public partial class FactionDetector : ComponentBehaviour
	{
		public FactionData FactionData { get; set; }
		[ReadOnly, ShowInInspector]
		public DetectedData DetectedData { get; set; }
		private DiplomacyComputer DiplomacyComputer { get; set; }
		private NavMeshConnectComputer NavMeshConnectComputer { get; set; }
		private MapCellData MapAICellData { get; set; }

		[Header("List Of DetectingDataType")]
		[ShowInInspector]
		public bool onShowListOfDetectingDataType { get; set; }
		[ShowInInspector, ReadOnly, ShowIf("@onShowListOfDetectingDataType")]
		public List<DetectedData.Info> MyFactionList { get; set; }
		[ShowInInspector, ReadOnly, ShowIf("@onShowListOfDetectingDataType")]
		public List<DetectedData.Info> AllianceList { get; set; }
		[ShowInInspector, ReadOnly, ShowIf("@onShowListOfDetectingDataType")]
		public List<DetectedData.Info> EnemyList { get; set; }
		[ShowInInspector, ReadOnly, ShowIf("@onShowListOfDetectingDataType")]
		public List<DetectedData.Info> NeutralList { get; set; }


		private List<int> searchedList;
		private List<DetectedData.Info> calculationList;
		private Dictionary<int, FactionDiplomacyType> factionDiplomacyTypeCash;


		private QuerySystem detectorQuerySystem;
		private OdccQueryCollector detectorQueryCollector;


		public override void BaseAwake()
		{
			FactionData = ThisObject.ThisContainer.TryGetData<FactionData>(out var factionData) ? factionData : null;
			DetectedData = ThisContainer.TryGetData<DetectedData>(out var fetectingData) ? fetectingData : null;
			//RangeComputing = ThisContainer.TryGetComponent<FactionDetectorRangeComputing>(out var rangeComputing) ? rangeComputing : null;

			if(ThisObject.ThisContainer.TryGetParentObject<LowLevelAIManager>(out var lowLevelAI))
			{
				DiplomacyComputer = lowLevelAI.ThisContainer.GetComponent<DiplomacyComputer>();
				NavMeshConnectComputer = lowLevelAI.ThisContainer.GetComponent<NavMeshConnectComputer>();
				MapAICellData = lowLevelAI.ThisContainer.GetData<MapCellData>();
			}

			MyFactionList = new List<DetectedData.Info>();
			AllianceList = new List<DetectedData.Info>();
			EnemyList = new List<DetectedData.Info>();
			NeutralList = new List<DetectedData.Info>();


			searchedList = new List<int>();
			calculationList = new List<DetectedData.Info>();
			factionDiplomacyTypeCash = new Dictionary<int, FactionDiplomacyType>();

			detectorQuerySystem = QuerySystemBuilder.CreateQuery()
				.WithAll<IFactionData, FireunitDetector>()
				.Build();
			detectorQueryCollector = null;
		}

		public override void BaseEnable()
		{
			base.BaseEnable();

			if(DiplomacyComputer is not null)
			{
				factionDiplomacyTypeCash.Add(FactionData.FactionIndex, FactionDiplomacyType.My_Faction);

				detectorQueryCollector = OdccQueryCollector.CreateQueryCollector(detectorQuerySystem)
					.CreateChangeListEvent(InitList, ChangeList)
					.CreateLooperEvent(nameof(FactionDetector))
					//.SetBreakFunction(() => !(NavMeshConnectComputer != null && NavMeshConnectComputer.IsAsyncUpdate))
					.CallNext(InitFactionDetector)
					.Foreach<FireunitDetector>(DetectorUpdate)//.SetFrameCount(() => 5)
					.Foreach<IFactionData, FireunitDetector>(DetectorComputing)//.SetFrameCount(() => 5)
					.CallNext(UpdateFactionDetector)
					.GetCollector();
			}
		}

		void InitList(IEnumerable<ObjectBehaviour> objectBehaviours)
		{
			foreach(ObjectBehaviour objectBehaviour in objectBehaviours)
			{
				if(objectBehaviour.ThisContainer.TryGetComponent<FireunitDetector>(out var component)
					&& objectBehaviour.ThisContainer.TryGetData<FactionData>(out var factionData)
					&& objectBehaviour.ThisContainer.TryGetData<FireteamData>(out var fireteamData))
				{
					Computing_SplitDetectionType(factionData, fireteamData, component, true);
				}
			}
		}
		void ChangeList(ObjectBehaviour objectBehaviour, bool isAdded)
		{
			if(objectBehaviour.ThisContainer.TryGetComponent<FireunitDetector>(out var component)
				&& objectBehaviour.ThisContainer.TryGetData<FactionData>(out var factionData)
				&& objectBehaviour.ThisContainer.TryGetData<FireteamData>(out var fireteamData))
			{
				Computing_SplitDetectionType(factionData, fireteamData, component, isAdded);
			}
		}

		public override void BaseDisable()
		{
			base.BaseDisable();

			if(detectorQueryCollector != null)
			{
				detectorQueryCollector
					.DeleteChangeListEvent(ChangeList)
					.DeleteLooperEvent(nameof(FactionDetector));
			}
		}

		public void InitFactionDetector()
		{
			calculationList.Clear();
			searchedList.Clear();
		}

		public void UpdateFactionDetector()
		{
			DetectedData.Clear();
			DetectedData.AddRange(calculationList);
		}
		public void Computing_SplitDetectionType(FactionData factionData, FireteamData fireteamData, FireunitDetector component, bool isAdded)
		{
			int targetFactionIndex = factionData.FactionIndex;
			FactionDiplomacyType factionDiplomacyType = FactionDiplomacyType.Neutral_Faction;
			if(FactionData.FactionIndex == targetFactionIndex)
			{
				factionDiplomacyType = FactionDiplomacyType.My_Faction;
			}
			else if(!factionDiplomacyTypeCash.TryGetValue(targetFactionIndex, out factionDiplomacyType))
			{
				factionDiplomacyType = DiplomacyComputer.GetFactionDiplomacyType(FactionData, factionData);
				factionDiplomacyTypeCash.Add(targetFactionIndex, factionDiplomacyType);
			}

			if(factionDiplomacyType == FactionDiplomacyType.My_Faction)
			{
				MyFactionDetection(factionDiplomacyType);
			}
			else if(factionDiplomacyType == FactionDiplomacyType.Alliance_Faction)
			{
				AllianceDetection(factionDiplomacyType);
			}
			else if(factionDiplomacyType == FactionDiplomacyType.Enemy_Faction)
			{
				EnemyDetection(factionDiplomacyType);
			}
			else //if(factionDiplomacyType == FactionDiplomacyType.Neutral_Faction)
			{
				NeutralDetection(factionDiplomacyType);
			}

			void SetCalculationList(ref List<DetectedData.Info> list, FactionData factionData, FireteamData fireteamData, FactionDiplomacyType factionDiplomacyType, FireunitDetector component)
			{
				if(isAdded)
				{
					list.Add(new DetectedData.Info() {
						detectorComponent = component,
						factionData = factionData,
						fireteamData = fireteamData,
						factionDiplomacyType = factionDiplomacyType,
					});
				}
				else
				{
					var remove = list.FirstOrDefault(item => item.detectorComponent == component);
					if(remove != null)
					{
						list.Remove(remove);
					}
				}
			}
			void MyFactionDetection(FactionDiplomacyType factionDiplomacyType)
			{
				var list = MyFactionList;
				SetCalculationList(ref list, factionData, fireteamData, factionDiplomacyType, component);
				MyFactionList = list;
			}
			void AllianceDetection(FactionDiplomacyType factionDiplomacyType)
			{
				var list = AllianceList;
				SetCalculationList(ref list, factionData, fireteamData, factionDiplomacyType, component);
				AllianceList = list;
			}
			void EnemyDetection(FactionDiplomacyType factionDiplomacyType)
			{
				var list = EnemyList;
				SetCalculationList(ref list, factionData, fireteamData, factionDiplomacyType, component);
				EnemyList = list;
			}
			void NeutralDetection(FactionDiplomacyType factionDiplomacyType)
			{
				var list = NeutralList;
				SetCalculationList(ref list, factionData, fireteamData, factionDiplomacyType, component);
				NeutralList = list;
			}
		}

		private void DetectorUpdate(OdccQueryLooper.LoopInfo loopInfo, FireunitDetector component)
		{
			component.UpdateDetector(NavMeshConnectComputer, MapAICellData);
		}
		public void DetectorComputing(OdccQueryLooper.LoopInfo loopInfo, IFactionData factionData, FireunitDetector component)
		{
			if(!factionDiplomacyTypeCash.TryGetValue(factionData.FactionIndex, out FactionDiplomacyType factionDiplomacyType)) return;

			if(factionDiplomacyType != FactionDiplomacyType.My_Faction)
			{
				return;
			}

			RaycastStart();

			int Count = EnemyList.Count;

			for(int i = 0 ; i < Count ; i++)
			{
				var enemyInfo = EnemyList[i];

				if(searchedList.Contains(i)) continue;

				FireunitDetector enemyComponent = enemyInfo.detectorComponent;

				if(!RangeComputing(component, enemyComponent))
				{
					continue;
				}

				if(!RaycastComputing(component, enemyComponent))
				{
					continue;
				}

				searchedList.Add(i);
				calculationList.Add(enemyInfo);
			}

			RaycastEnded();

		}

		public void OnChangeDetectorList(List<FireunitDetector> addedList, List<FireunitDetector> removeList)
		{

		}
	}
}
