using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The WindowView is responsible for the layout and GUI of the EditorWindow.
/// It represents the View in the MVC pattern.
/// </summary>
public class WindowView : EditorWindow
{
    public ObjectSettings ObjectSettings { get; set; }
    
    private SettingsSection _settingsSection;
    private SidebarSection _sidebarSection;
    private WindowController _controller;
    private ObjectGenerator _object;
    
    public SerializedObject SerializedObject;
    
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
    public string SettingsHeader;
    public float SidebarFrameWidth;

    public WindowView()
    {
        _sidebarSection = new SidebarSection(this);
        _settingsSection = new SettingsSection(this);
    }

    private void OnEnable()
    {
        ObjectSettings = FindObjectOfType<ObjectGenerator>().ObjectSettings;

        SerializedObject = new SerializedObject(ObjectSettings);
        ObjectTypeProperty = SerializedObject.FindProperty("ObjectType");
        MaterialProperty = SerializedObject.FindProperty("Material");
        ResolutionProperty = SerializedObject.FindProperty("Resolution");
        RadiusProperty = SerializedObject.FindProperty("Radius");
        GradientProperty = SerializedObject.FindProperty("Gradient");
        NoiseLayerProperty  = SerializedObject.FindProperty("NoiseLayers");
        
        _sidebarSection = new SidebarSection(this);
        _settingsSection = new SettingsSection(this);
        
        _controller = new WindowController();
        _controller.GUIChanged += GUIChanged;
        _controller.DrawSideBarSection += DrawSideBarSection;
        _controller.DrawSettingsSection += DrawSettingsSection;
        _controller.UpdateSerializedObject += UpdateSerializedObject;
        _controller.AssetNamesAndPathsReady += GetAllAssetNamesAndPaths;
        _controller.OnGetAllAssetNamesAndPaths();
        
        UpdateSettingsSectionHeader();
    }

    private void OnDisable()
    {
        _controller = null;
    }

    [MenuItem("Tools/Planet Generator")]
    public static void ShowWindow()
    {
        GetWindow<WindowView>(TextHolder.WindowTitle);
    }

    private void OnGUI()
    {
        OnUpdate();
        DrawUI();
        OnGUIChanged();
    }

    private void DrawUI()
    {
        GUILayout.BeginHorizontal();
        _controller.OnDrawSideBarSection(AssetsInFolder);
        _controller.OnDrawSettingsSection();
        GUILayout.EndHorizontal();
    }
    
    private void OnUpdate()
    {
        _controller.OnUpdate();
    }
    
    private void OnGUIChanged()
    {
        _controller.OnGUIChanged();
    }

    #region EditorWindow Events
    
    private void UpdateSerializedObject()
    {
        SerializedObject.Update();
    }

    private void GUIChanged()
    {
        if (GUI.changed)
        {
            SerializedObject.ApplyModifiedProperties();
            UpdateSettingsSectionHeader();
        }
    }

    #endregion
    
    #region Event Handlers

    private void GUIChanged(object sender, EventArgs e)
    {
        GUIChanged();
        Repaint();
    }

    private void UpdateSerializedObject(object sender, EventArgs e)
    {
        UpdateSerializedObject();
    }
    
    private void DrawSideBarSection(object sender, EventArgs e)
    {
        _sidebarSection.DrawSideBarSection();
    }
    
    private void DrawSettingsSection(object sender, EventArgs e)
    {
        _settingsSection.DrawSettingsSection();
    }
    
    private void GetAllAssetNamesAndPaths((string name, string path)[] assetNamesAndPaths, EventArgs e)
    {
        AssetsInFolder = assetNamesAndPaths;
        Repaint();
    }

    #endregion
    
    private void UpdateSettingsSectionHeader()
    {
        SettingsHeader = $"Settings [{ObjectSettings.name}]";
    }

    public float SetSettingsSectionWidth()
    {
        return position.width - SidebarFrameWidth - 10;
    }
    
    public void DrawSaveButton()
    {
        if (GUILayout.Button(TextHolder.UpdateButtonText))
        {
            EditorUtility.SetDirty(ObjectSettings);
            AssetDatabase.SaveAssets();
        }
    }
}