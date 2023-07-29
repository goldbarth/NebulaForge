using Object = UnityEngine.Object;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ObjectGenerator))]
public class ObjectEditor : Editor
{
    private ObjectSettings _oldSettings;
    private ObjectGenerator _object;
    private Editor _shapeEditor;

    private string[] _assetNames;
    private string _assetName = string.Empty;
    private int _selectedAssetIndex;
    private int _oldSelectedAssetIndex;

    private void OnEnable()
    {
        _object = (ObjectGenerator)target;
        _oldSettings = _object.ObjectSettings;
        _assetNames = GetAssetNamesInFolder();
        //_selectedAssetIndex = Array.IndexOf(_assetNames, _object.ObjectSettings.name);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        using var check = new EditorGUI.ChangeCheckScope();

        ButtonLayout();
        DrawSettingsEditor(_object.ObjectSettings, _object.OnPlanetSettingsUpdated, _object.OnColorSettingsUpdated,
            ref _object.ShapeSettingsFoldout, ref _shapeEditor);
        
        OnChanged(check);
    }

    private void OnChanged(EditorGUI.ChangeCheckScope check)
    {
        if (check.changed)
        {
            _object.GeneratePlanet();
            //_selectedAssetIndex = Array.IndexOf(_assetNames, _object.ObjectSettings.name);
        }
    }

    private void ButtonLayout()
    {
        GUILayout.Label("Object Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        var applySelectionTooltip =
            "Select the asset you want to use. After selecting the asset. " +
            "The settings, material and texture will automatically applied to the object.";
        var newSelectedAssetIndex = 
            DropdownWithTooltip("Select Asset", applySelectionTooltip, _selectedAssetIndex, _assetNames);

        // Check if the selected index has changed. If so, apply the new settings.
        if (newSelectedAssetIndex != _selectedAssetIndex)
        {
            _selectedAssetIndex = newSelectedAssetIndex;
            ApplySelectedAsset();
        }

        GUILayout.Space(2);
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        if (GUILayout.Button("Generate Planet"))
        {
            _object.GeneratePlanet();
            _object.OnColorSettingsUpdated();
        }

        GUILayout.Space(2);
        if (GUILayout.Button("Remove Planet"))
            _object.RemovePlanet();

        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        GUILayout.Space(2);
        GUILayout.Label("Create New Object Settings Asset", EditorStyles.boldLabel);
        GUILayout.Space(5);
        _assetName = EditorGUILayout.TextField("Asset Name", _assetName);
        GUILayout.Space(3);

        if (GUILayout.Button("Create Asset"))
            CreateNewAsset(_object);
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        GUILayout.Space(2);
        GUILayout.Label("Delete Object Settings Asset", EditorStyles.boldLabel);
        GUILayout.Space(5);
        var deleteSelectionTooltipText = "Select the asset you want to delete. After selecting the asset, click the Delete Asset button.";
        _oldSelectedAssetIndex = DropdownWithTooltip("Select Asset", deleteSelectionTooltipText, _oldSelectedAssetIndex, _assetNames);
        GUILayout.Space(3);

        if (GUILayout.Button("Delete Asset"))
            DeleteSelectedAsset(_assetNames);
    }

    private int DropdownWithTooltip(string label, string tooltipText, int selectedIndex, string[] assetNames)
    {
        return EditorGUILayout.Popup(new GUIContent(label, tooltipText), selectedIndex, assetNames);
    }

    private void ApplySelectedAsset()
    {
        var asset = SetSelected(_assetNames);
        var fullPath = $"{asset.path}.asset";

        var selectedAsset = AssetDatabase.LoadAssetAtPath<ObjectSettings>(fullPath);
        if (selectedAsset == null)
        {
            Debug.LogWarning("Invalid asset path to load from.");
            return;
        }

        _object.ObjectSettings = selectedAsset;
        _object.GeneratePlanet();
    }

    private void DeleteSelectedAsset(string[] assetNames)
    {
        var asset = SetSelected(assetNames);
        AssetDatabase.DeleteAsset($"{asset.folderPath}");
        Debug.Log($"Deleted assets and folder at: {asset.folderPath}.");

        AssetDatabase.Refresh();
    }

    // Using tuples to return multiple values, checking edge cases and to return only individual values if needed. And for fun ofc.
    // https://learn.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/value-tuples
    private (string folderPath, string path) SetSelected(string[] assetNames)
    {
        if (_selectedAssetIndex < 0 || _selectedAssetIndex >= assetNames.Length)
            Debug.LogWarning("Invalid asset index.");

        var selectedAssetName = assetNames[_selectedAssetIndex];
        var folderPath = FolderPath.NewAssetFolder(selectedAssetName);
        var path = $"{folderPath}/{selectedAssetName}";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Invalid asset folder path.");
            folderPath = string.Empty;
        }

        return (folderPath, path);
    }

    private string[] GetAssetNamesInFolder()
    {
        // Fetch all the asset paths in the folder we want to choose assets from.
        const string folderPath = FolderPath.RootInstances;
        var assetPaths = AssetDatabase.FindAssets("t:ObjectSettings", new[] { folderPath });

        // Convert asset paths to asset names.
        var assetNames = new string[assetPaths.Length];
        for (var assetIndex = 0; assetIndex < assetPaths.Length; assetIndex++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[assetIndex]);
            assetNames[assetIndex] = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        }

        return assetNames;
    }

    private void CreateNewAsset(ObjectGenerator objectGenerator)
    {
        var newAssetFolder = FolderPath.NewAssetFolder(_assetName);
        if (!string.IsNullOrEmpty(_assetName))
        {
            if (AssetDatabase.IsValidFolder(newAssetFolder))
            {
                Debug.LogError("An asset with the same name already exists.");
                return;
            }

            if (!System.IO.Directory.Exists(newAssetFolder))
                System.IO.Directory.CreateDirectory(newAssetFolder);

            var newObjectSettings = CreateNewInstance();
            CreateAndSaveAsset(objectGenerator, newObjectSettings, newAssetFolder);
            ResetAssetData();
        }
        else
        {
            Debug.LogWarning("Asset name can´t be empty! You need to enter a name for the asset.");
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
        var newObjectSettings = CreateInstance<ObjectSettings>();
        if (_oldSettings == null) return newObjectSettings;

        // Make a copy of the current settings and material and assign it to the object.
        newObjectSettings = Instantiate(_oldSettings);
        var newMaterial = Instantiate(_oldSettings.Material);
        newObjectSettings.Material = newMaterial;

        return newObjectSettings;
    }

    private static void DrawSettingsEditor(Object planetSettings, Action callbackShapeSettings,
        Action callbackColorSettings, ref bool foldout, ref Editor editor)
    {
        if (planetSettings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, planetSettings);
            using var check = new EditorGUI.ChangeCheckScope();
            if (foldout)
            {
                CreateCachedEditor(planetSettings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    callbackShapeSettings?.Invoke();
                    callbackColorSettings?.Invoke();
                }
            }
        }
    }
}