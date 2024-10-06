#if UNITY_EDITOR
using Sirenix.OdinInspector;

using UnityEditor;
#endif

using UnityEngine;

namespace BC.GamePlayerManager
{
	[CreateAssetMenu(fileName = "GamePlaySetting", menuName = "BC/StartPlaySetting/new GamePlaySetting")]
	public partial class MainPlaySetting : ScriptableObject
	{
		[HideLabel]
		public FactionSetting FactionSetting;
		[HideLabel]
		public MapSetting MapSetting;
		[HideLabel]
		public TeamSetting TeamSetting;
		[HideLabel]
		public UnitSetting UnitSetting;
		[HideLabel]
		public CharacterSetting CharacterSetting;
#if UNITY_EDITOR
		private void Reset()
		{
			OnValidate();
		}
		private void OnValidate()
		{
			// 각 필드에 대해 생성 확인 및 반환
			FactionSetting = FactionSetting != null ? FactionSetting : CreateSubSetting<FactionSetting>();
			MapSetting = MapSetting != null ? MapSetting : CreateSubSetting<MapSetting>();
			UnitSetting = UnitSetting != null ? UnitSetting : CreateSubSetting<UnitSetting>();
			TeamSetting = TeamSetting != null ? TeamSetting : CreateSubSetting<TeamSetting>();
			CharacterSetting = CharacterSetting != null ? CharacterSetting : CreateSubSetting<CharacterSetting>();
			if(EditorUtility.IsDirty(this))
			{
				// 에셋 데이터베이스 저장 및 갱신
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				// 프로젝트 창에서 생성된 부모 에셋 선택
				Selection.activeObject = this;
			}
		}

		public T CreateSubSetting<T>() where T : SubPlaySetting
		{
			// 동일 타입의 기존 자식 에셋이 있는 경우 해당 에셋을 반환
			T existingChild = FindSubSetting<T>();
			if(existingChild != null)
			{
				if(existingChild.name != typeof(T).Name)
				{
					existingChild.name = typeof(T).Name;
					EditorUtility.SetDirty(existingChild);
					EditorUtility.SetDirty(this);
				}
				return existingChild;
			}

			// 새 자식 인스턴스 생성
			T child = ScriptableObject.CreateInstance<T>();
			child.name = typeof(T).Name;
			child.mainPlaySetting = this;

			// 자식 에셋을 MainPlaySetting의 하위로 추가
			AssetDatabase.AddObjectToAsset(child, this);
			EditorUtility.SetDirty(this);

			return child;
		}

		// 기존 자식 에셋 찾기
		private T FindSubSetting<T>() where T : SubPlaySetting
		{
			// MainPlaySetting에 속한 모든 하위 에셋 검색
			Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			foreach(Object subAsset in subAssets)
			{
				// 동일한 타입의 자식 에셋이 있는지 확인
				if(subAsset is T existing)
				{
					return existing;
				}
			}
			return null;
		}
#endif
	}
}
