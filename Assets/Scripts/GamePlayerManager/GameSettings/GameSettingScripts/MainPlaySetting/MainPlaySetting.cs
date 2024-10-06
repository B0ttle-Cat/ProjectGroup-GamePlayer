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
			// �� �ʵ忡 ���� ���� Ȯ�� �� ��ȯ
			FactionSetting = FactionSetting != null ? FactionSetting : CreateSubSetting<FactionSetting>();
			MapSetting = MapSetting != null ? MapSetting : CreateSubSetting<MapSetting>();
			UnitSetting = UnitSetting != null ? UnitSetting : CreateSubSetting<UnitSetting>();
			TeamSetting = TeamSetting != null ? TeamSetting : CreateSubSetting<TeamSetting>();
			CharacterSetting = CharacterSetting != null ? CharacterSetting : CreateSubSetting<CharacterSetting>();
			if(EditorUtility.IsDirty(this))
			{
				// ���� �����ͺ��̽� ���� �� ����
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				// ������Ʈ â���� ������ �θ� ���� ����
				Selection.activeObject = this;
			}
		}

		public T CreateSubSetting<T>() where T : SubPlaySetting
		{
			// ���� Ÿ���� ���� �ڽ� ������ �ִ� ��� �ش� ������ ��ȯ
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

			// �� �ڽ� �ν��Ͻ� ����
			T child = ScriptableObject.CreateInstance<T>();
			child.name = typeof(T).Name;
			child.mainPlaySetting = this;

			// �ڽ� ������ MainPlaySetting�� ������ �߰�
			AssetDatabase.AddObjectToAsset(child, this);
			EditorUtility.SetDirty(this);

			return child;
		}

		// ���� �ڽ� ���� ã��
		private T FindSubSetting<T>() where T : SubPlaySetting
		{
			// MainPlaySetting�� ���� ��� ���� ���� �˻�
			Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			foreach(Object subAsset in subAssets)
			{
				// ������ Ÿ���� �ڽ� ������ �ִ��� Ȯ��
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
