# if UNITY_EDITOR

using PlanetSettings;
using UnityEditor;
using UnityEngine;
using HelpersAndExtensions;

namespace CustomEditorWindow.Dependencies
{
    public class CreateNewAsset
    {
        private ObjectSettings _currentSettings;
    
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        private Material _material;
        
        // Celestial object creation related method
        public ObjectSettings CreateAsset(ObjectSettings newSettings ,ObjectType objectType, string assetName = "")
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("Asset name cannot be empty.");
                return null;
            }
    
            var newAssetFolder = FolderPath.NewAssetFolder(assetName);
            if (AssetDatabase.IsValidFolder(newAssetFolder))
            {
                Debug.LogError("An asset with the same name already exists.");
                return null;
            }

            if (!System.IO.Directory.Exists(newAssetFolder))
                System.IO.Directory.CreateDirectory(newAssetFolder);
            
            SetMaterial(newSettings, objectType);
            CreateAndSaveAssets(newSettings, assetName, newAssetFolder);

            return newSettings;
        }

        private void SetMaterial(ObjectSettings newSettings, ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.SolidSphere:
                    _material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    break;
                case ObjectType.TerrestrialBody:
                    _material = new Material(Shader.Find("Shader Graphs/TerrainGradient_SG"));
                    break;
            }

            newSettings.Material = _material;
        }

        private void CreateAndSaveAssets(ObjectSettings newSettings, string assetName, string newAssetFolder)
        {
            AssetDatabase.CreateAsset(newSettings, $"{newAssetFolder}/{assetName}.asset");
            AssetDatabase.CreateAsset(newSettings.Material, $"{newAssetFolder}/{assetName}Material.asset");
            SaveTexture(newSettings, newAssetFolder, assetName);
        }

        private void SaveTexture(ObjectSettings newObjectSettings, string newAssetFolder, string assetName)
        {
            var originTexture = newObjectSettings.Material.GetTexture(MainTex) as Texture2D;
            var savePath = $"{newAssetFolder}/{assetName}Texture.png";
            var activeRenderTexture = RenderTexture.active;

            // Because the texture is readonly and it is not possible to change every new created texture in the editor,
            // we need to get the raw texture data from the original texture and save it as a new texture.
            // Get the raw texture data from the original texture.
            if (originTexture != null)
            {
                var tempTexture = RenderTexture.GetTemporary(originTexture.width, originTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                // Copy the original texture to the temporary texture.
                Graphics.Blit(originTexture, tempTexture); 
                RenderTexture.active = tempTexture;
                var textureCopy = new Texture2D(originTexture.width, originTexture.height, TextureFormat.RGBA32, false);
                textureCopy.ReadPixels(new Rect(0, 0, tempTexture.width, tempTexture.height), 0, 0);
                textureCopy.Apply();
                // Ensure that the active render texture is the original one.
                RenderTexture.active = activeRenderTexture;
                RenderTexture.ReleaseTemporary(tempTexture);
        
                var textureBytes = textureCopy.EncodeToPNG();
                System.IO.File.WriteAllBytes(savePath, textureBytes);
            }

            AssetDatabase.Refresh();
        }
    }
}

#endif

