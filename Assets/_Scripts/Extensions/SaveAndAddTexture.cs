using PlanetSettings;
using UnityEditor;
using UnityEngine;

namespace Extensions
{
    public static class SaveAndAddTexture
    {
        public static void SaveTextureToFile(this Texture2D texture, ObjectSettings currentAsset)
        {
            var folderPath = FolderPath.NewAssetFolder(currentAsset.name);
        
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            // save texture to file in the folder we just created
            if (currentAsset != null)
            {
                var fullSavePath = GetSavePath(currentAsset.name);
                var textureBytes = texture.EncodeToPNG();
                System.IO.File.WriteAllBytes(fullSavePath, textureBytes);
            }

            // refresh asset database to make sure the file we just saved shows up in the project
            AssetDatabase.Refresh();
        }

        public static void AddTexture(this Material material, ObjectSettings asset)
        {
            // add the saved texture to the material
            var newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetSavePath(asset.name));
            material.SetTexture("_Planet_Texture", newTexture);
        }

        private static string GetSavePath(string assetName)
        {
            var newAssetFolder = FolderPath.NewAssetFolder(assetName);
            return $"{newAssetFolder}/{assetName}Texture.png";
        }
    }
}