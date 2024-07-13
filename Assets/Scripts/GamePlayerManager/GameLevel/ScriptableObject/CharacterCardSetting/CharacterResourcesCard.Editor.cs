#if UNITY_EDITOR
using System.IO;
using System.Linq;

using UnityEditor;

namespace BC.GamePlayerManager
{
	public partial class CharacterResourcesCard//.Editor
	{
		public void UpdateAssetName()
		{
			if(string.IsNullOrWhiteSpace(characterName)) return;

			string assetPath = AssetDatabase.GetAssetPath(this);
			string directory = Path.GetDirectoryName(assetPath);
			string newAssetName = $"{characterName} CharacterCard";

			// ������ Ÿ���� ��� ������ ã��
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(CharacterResourcesCard).Name}", new[] { directory });
			string[] assetNames = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
										.Select(path => Path.GetFileNameWithoutExtension(path))
										.ToArray();

			// ������ �̸��� �̹� �����ϴ��� Ȯ��
			if(!assetNames.Contains(newAssetName))
			{
				// ������ �̸��� �������� �ʴ� ��쿡�� ���� �̸� ������ ���� ����
				if(Path.GetFileNameWithoutExtension(assetPath) != newAssetName)
				{
					AssetDatabase.RenameAsset(assetPath, newAssetName);
					AssetDatabase.SaveAssets();
				}
			}
		}
	}
}
#endif