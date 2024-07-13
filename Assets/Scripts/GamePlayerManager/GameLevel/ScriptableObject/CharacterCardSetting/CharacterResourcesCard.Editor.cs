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

			// 동일한 타입의 모든 에셋을 찾음
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(CharacterResourcesCard).Name}", new[] { directory });
			string[] assetNames = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
										.Select(path => Path.GetFileNameWithoutExtension(path))
										.ToArray();

			// 동일한 이름이 이미 존재하는지 확인
			if(!assetNames.Contains(newAssetName))
			{
				// 동일한 이름이 존재하지 않는 경우에만 에셋 이름 변경을 지연 실행
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