# if UNITY_EDITOR

using PlanetSettings;
using UnityEditor;
using UnityEngine;
using HelpersAndExtensions;
using Planet;

namespace CustomEditorWindow.Dependencies
{
    public class CreateNewAsset
    {
        private ObjectSettings _currentSettings;
    
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        public bool IsAssetNameEmpty;
        public bool IsAssetNameValid = true;
    
        private string _assetName = string.Empty;

        public void CreateAsset(ObjectGenerator objectGenerator, WindowLayout layout, string assetName = "")
        {
            _currentSettings = objectGenerator.ObjectSettings;
            _assetName = assetName;
            var newAssetFolder = FolderPath.NewAssetFolder(_assetName);
            if (!string.IsNullOrEmpty(_assetName))
            {
                IsAssetNameEmpty = false;
                if (AssetDatabase.IsValidFolder(newAssetFolder))
                {
                    IsAssetNameValid = false;
                    Debug.LogError("An asset with the same name already exists.");
                    return;
                }
            
                IsAssetNameValid = true;

                if (!System.IO.Directory.Exists(newAssetFolder))
                    System.IO.Directory.CreateDirectory(newAssetFolder);

                var newObjectSettings = CreateNewInstance();
                CreateAndSaveAsset(objectGenerator, newObjectSettings, newAssetFolder);
                SaveTexture(newObjectSettings, newAssetFolder);
            
                // Update the asset list in the window.
                layout.SetAllAssetsInFolder();
            
                ResetAssetData();
            }
            else
            {
                IsAssetNameEmpty = true;
            }
        }

        private void ResetAssetData()
        {
            _assetName = string.Empty;
            _currentSettings = null;
        }

        private void CreateAndSaveAsset(ObjectGenerator objectGenerator, ObjectSettings newObjectSettings, string newAssetFolder)
        {
            AssetDatabase.CreateAsset(newObjectSettings, $"{newAssetFolder}/{_assetName}.asset");
            AssetDatabase.CreateAsset(newObjectSettings.Material, $"{newAssetFolder}/{_assetName}Material.asset");

            objectGenerator.ObjectSettings = newObjectSettings;
            EditorUtility.SetDirty(objectGenerator);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created new asset at: {newAssetFolder}");
        }

        private void SaveTexture(ObjectSettings newObjectSettings, string newAssetFolder)
        {
            var originTexture = newObjectSettings.Material.GetTexture(MainTex) as Texture2D;
            var savePath = $"{newAssetFolder}/{_assetName}Texture.png";
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

        private ObjectSettings CreateNewInstance()
        {
            var newObjectSettings = ScriptableObject.CreateInstance<ObjectSettings>();
            var originMaterial = _currentSettings.Material;

            if (_currentSettings == null) return newObjectSettings;

            // Make a copy of the current settings and material and assign it to the object.
            newObjectSettings = UnityEngine.Object.Instantiate(_currentSettings);
            var newMaterial = UnityEngine.Object.Instantiate(originMaterial);
            newObjectSettings.Material = newMaterial;

            return newObjectSettings;
        }
    }
}

#endif

