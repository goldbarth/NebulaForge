using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine;

public class CreateNewAsset
{
    private ObjectSettings _oldSettings;
    
    public bool IsAssetNameEmpty;
    public bool IsAssetNameValid = true;
    
    private string _assetName = string.Empty;

    public void CreateAsset(ObjectGenerator objectGenerator, string assetName = "")
    {
        _oldSettings = objectGenerator.ObjectSettings;
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
        _oldSettings = null;
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

    private ObjectSettings CreateNewInstance()
    {
        var newObjectSettings = ScriptableObject.CreateInstance<ObjectSettings>();
        if (_oldSettings == null) return newObjectSettings;

        // Make a copy of the current settings and material and assign it to the object.
        newObjectSettings = Object.Instantiate(_oldSettings);
        var newMaterial = Object.Instantiate(_oldSettings.Material);
        newObjectSettings.Material = newMaterial;

        return newObjectSettings;
    }
}