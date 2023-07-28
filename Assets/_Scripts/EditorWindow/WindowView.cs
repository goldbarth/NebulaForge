using UnityEditor;
using UnityEngine;
using System;

#if UNITY_EDITOR
/// <summary>
/// The WindowView is responsible for the layout and GUI of the EditorWindow.
/// It represents the View in the MVC pattern.
/// </summary>
public class WindowView : EditorWindow
{
    public ObjectGenerator ObjectGenerator{ get; private set; }
    public ObjectSettings ObjectSettings { get; set; }
    public string SettingsHeader { get; set; }

    private WindowController _controller;
    private SettingsSection _settingsSection;
    private SidebarSection _sidebarSection;

    private SerializedObject _serializedObject;
    
    public SerializedProperty MaterialProperty;
    public SerializedProperty ResolutionProperty;
    public SerializedProperty RadiusProperty;
    public SerializedProperty GradientProperty;
    public SerializedProperty ObjectTypeProperty;
    public SerializedProperty NoiseLayerProperty;
    public SerializedProperty EnabledProperty;
    public SerializedProperty UseFirstLayerAsMaskProperty;
    public SerializedProperty NoiseSettingsProperty;
    
    public (string name, string path)[] AssetsInFolder;
    public float SidebarFrameWidth;

    private void OnEnable()
    {
        FindAndSetObjectSettings();
        
        _settingsSection = new SettingsSection(this);
        _sidebarSection = new SidebarSection(this);
        _controller = new WindowController();

        _controller.OnDrawUI += DrawLayout;
        _controller.OnGUIChanged += UpdateSettings;
        _controller.OnApplyModified += ApplyModifiedProperties;
        _controller.OnAssetNamesAndPathsReady += SetAssetNamesAndPaths;
        
        ObjectSelectionEventManager.OnObjectSelected += InitializeProperties;
        ObjectSelectionEventManager.OnNoObjectSelected += SetObjectSettingNull;

        if (ObjectSettings == null) return;
        InitializeProperties();
    }

    private void OnDisable()
    {
        if (_controller == null) return;
        _controller.OnDrawUI -= DrawLayout;
        _controller.OnGUIChanged -= UpdateSettings;
        _controller.OnApplyModified -= ApplyModifiedProperties;
        _controller.OnAssetNamesAndPathsReady -= SetAssetNamesAndPaths;
        
        ObjectSelectionEventManager.OnObjectSelected -= InitializeProperties;
        ObjectSelectionEventManager.OnNoObjectSelected -= SetObjectSettingNull;
        
        _controller = null;
    }

    [MenuItem("Tools/Planet Generator")]
    public static void ShowWindow()
    {
        GetWindow<WindowView>(TextHolder.WindowTitle);
    }

    private void OnGUI()
    {
        if (ObjectSettings == null)
        {
            NoObjectSelectedMessage();
            return;
        }
        GUIChanged();
        DrawUI();
        ApplyAndModify();
    }

    private void OnSelectionChange()
    {
        FindAndSetObjectSettings();
        SetSerializedProperties();
    }

    #region Controller Dependencies

    private void DrawUI()
    {
        _controller.DrawUI();
    }
    
    private void GUIChanged()
    {
        if (GUI.changed)
        {
            _controller.GUIChanged();
        }
    }

    private void ApplyAndModify()
    {
        _controller.ApplyAndModify();
    }

    public void SetAllAssetsInFolder()
    {
        _controller.SetAllAssetsInFolder();
    }

    #endregion
    
    #region Wrapper Methods
    
    private void DrawLayout()
    {
        GUILayout.BeginHorizontal();
        DrawSideBarSection();
        DrawSettingsSection();
        GUILayout.EndHorizontal();
    }
    
    private void UpdateSettings()
    {
        UpdateSerializedObject();
        SetObjectGeneratorSettings();
        Repaint();
    }
    
    private void InitializeProperties()
    {
        SetSettingsSectionWidth();
        SetSettingsSectionHeader();
        SetAllAssetsInFolder();
        if (_serializedObject == null) return;
        SetSerializedProperties();
    }
    
    private void SetObjectSettings()
    {
        ObjectSettings = ObjectGenerator.ObjectSettings;
    }
    
    private void SetObjectGeneratorSettings()
    {
        ObjectGenerator.ObjectSettings = ObjectSettings;
    }

    private void ApplyModifiedProperties()
    {
        _serializedObject.ApplyModifiedProperties();
    }
    
    private void SetAssetNamesAndPaths()
    {
        AssetsInFolder = GetAssetNamesAndPaths();
    }

    private void DrawSideBarSection()
    {
        SetAllAssetsInFolder();
        _sidebarSection.DrawSideBarSection();
    }
    
    private void DrawSettingsSection()
    {
        _settingsSection.DrawSettingsSection();
    }

    #endregion
    
    private void SetObjectSettingNull()
    {
        ObjectSettings = null;
    }

    public ObjectSettings SetSelectedAsset((string name, string path) asset)
    {
        return AssetDatabase.LoadAssetAtPath<ObjectSettings>(asset.path);
    }
    
    public float SetSettingsSectionWidth()
    {
        return position.width - SidebarFrameWidth - 10;
    }
    
    public void SetNoiseLayerProperty(int layerIndex)
    {
        NoiseLayerProperty = _serializedObject.FindProperty("NoiseLayers").GetArrayElementAtIndex(layerIndex);
    }

    private void FindAndSetObjectSettings()
    {
        ObjectGenerator = FindSelectedObjectGetComponent();
        if(ObjectGenerator == null) return;
        SetObjectSettings();
    }

    private void UpdateSerializedObject()
    {
        _serializedObject.Update();
    }

    private void SetSettingsSectionHeader()
    {
        if (ObjectSettings == null) return;
        SettingsHeader = $"Settings [{ObjectSettings.name}]";
    }
    
    private static void NoObjectSelectedMessage()
    {
        EditorGUILayout.HelpBox("No GameObject was selected in the Hierarchy. Please select a GameObject to access the settings.",
            MessageType.Info);
    }

    public void UpdateNoiseLayerArray(NoiseLayer[] newLayers)
    {
        ObjectSettings.NoiseLayers = newLayers;
        _serializedObject = new SerializedObject(ObjectSettings);
    }
    
    private void SetSerializedProperties()
    {
        _serializedObject = new SerializedObject(ObjectSettings);
        ObjectTypeProperty = _serializedObject.FindProperty("ObjectType");
        MaterialProperty = _serializedObject.FindProperty("Material");
        ResolutionProperty = _serializedObject.FindProperty("Resolution");
        RadiusProperty = _serializedObject.FindProperty("Radius");
        GradientProperty = _serializedObject.FindProperty("Gradient");
        NoiseLayerProperty = _serializedObject.FindProperty("NoiseLayers");
    }

    public void AttachDataToAsset(ObjectSettings selectedAsset)
    {
        ObjectGenerator.ObjectSettings = selectedAsset;
        SetObjectSettings();
        SetSettingsSectionHeader();
        SetSerializedProperties();
        SetAllAssetsInFolder();

        EditorUtility.SetDirty(ObjectSettings);
        ObjectGenerator.GeneratePlanet();
    }
    
    private ObjectGenerator FindSelectedObjectGetComponent()
    {
        var selectedGameObject = Selection.activeGameObject;

        if (selectedGameObject != null)
        {
            var script = selectedGameObject.GetComponent<ObjectGenerator>();
            if (script != null) return script;

            var scriptsInChildren = selectedGameObject.GetComponentsInChildren<ObjectGenerator>();
            if (scriptsInChildren.Length > 0) return scriptsInChildren[0];
        }
        else
        {
            ObjectGenerator = null;
            Debug.LogWarningFormat("No GameObject selected in the Hierarchy. Please select a GameObject first. This happened at: {0:f}. Isn´t it cool?", DateTime.Now);
        }
        
        return null;
    }
    
    /// <summary>
    /// Tuple with asset name and asset path. Less than three.
    /// </summary>
    /// <returns>Returns a tuple array with all asset. Each tuple contains an asset name and path.</returns>
    private (string name, string path)[] GetAssetNamesAndPaths()
    {
        // Get all asset paths from the folder.
        const string folderPath = FolderPath.RootInstances;
        var assetPaths = AssetDatabase.FindAssets("t:ObjectSettings", new[] { folderPath });
    
        // Create a tuple array with the asset name and path.
        var assetNamesAndPaths = new (string name, string path)[assetPaths.Length];
        
        // Fill the tuple array with the asset name and path.
        for (var assetIndex = 0; assetIndex < assetPaths.Length; assetIndex++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[assetIndex]);
            var assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            assetNamesAndPaths[assetIndex] = (assetName, assetPath);
        }
        
        return assetNamesAndPaths;
    }
}
#endif