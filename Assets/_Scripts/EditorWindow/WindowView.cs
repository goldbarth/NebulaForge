using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The WindowView is responsible for the layout and GUI of the EditorWindow.
/// It represents the View in the MVC pattern.
/// </summary>
public class WindowView : EditorWindow
{
    private SerializedObject _serializedObject;
    private ObjectSettings _objectSettings;
    private WindowController _controller;
    private ObjectGenerator _object;

    private SerializedProperty _objectTypeProperty;
    private SerializedProperty _materialProperty;
    private SerializedProperty _resolutionProperty;
    private SerializedProperty _radiusProperty;
    private SerializedProperty _gradientProperty;
    
    private SerializedProperty _noiseLayerProperty;
    private SerializedProperty _enabledProperty;
    private SerializedProperty _useFirstLayerAsMaskProperty;
    private SerializedProperty _noiseSettingsProperty;

    private const string WindowTitle = "Planet Generator";
    private const string SidebarHeader = "Objects";
    private const string SidebarSubHeader1 = "Terrestrial Bodies";
    private const string SidebarSubHeader2= "Solid Spheres";
    private const string GeneralSettingsHeader = "General";
    private const string SurfaceSettingsHeader = "Surface";
    private const string ElevationLayerSettingsHeader = "Elevation Layer Settings";
    
    private readonly string[] _tabHeaders = { "General", "Surface" };
    
    private readonly string _currentAssetLabel = "Asset";
    private readonly string _objectTypeLabel = "Surface Shape";
    
    private readonly string _addLayerButton = "Add Layer";
    private readonly string _removeLayerButton = "Remove Layer";
    private readonly string _updateButton = "Update";
    
    private string _settingsHeader;
    
    private Vector2 _rightScrollPosition;
    private Vector2 _leftScrollPosition;
    
    private float _sidebarFrameWidth;
    private int _tabIndex;
    
    public WindowView(WindowController controller)
    {
        _controller = controller;
    }

    private void OnEnable()
    {
        _object = FindObjectOfType<ObjectGenerator>();
        _objectSettings = _object.ObjectSettings;
        
        UpdateSettingsSectionHeader();

        _serializedObject = new SerializedObject(_objectSettings);
        _objectTypeProperty = _serializedObject.FindProperty("ObjectType");
        _materialProperty = _serializedObject.FindProperty("Material");
        _resolutionProperty = _serializedObject.FindProperty("Resolution");
        _radiusProperty = _serializedObject.FindProperty("Radius");
        _gradientProperty = _serializedObject.FindProperty("Gradient");
        _noiseLayerProperty  = _serializedObject.FindProperty("NoiseLayers");
        
        _controller = new WindowController(this);
        _controller.GUIChanged += GUIChanged;
        _controller.UpdateSerializedObject += UpdateSerializedObject;
        _controller.DrawSideBarSection += DrawSideBarSection;
        _controller.DrawSettingsSection += DrawSettingsSection;
    }

    private void OnDisable()
    {
        _controller.GUIChanged -= GUIChanged;
        _controller.UpdateSerializedObject -= UpdateSerializedObject;
        _controller.DrawSideBarSection -= DrawSideBarSection;
        _controller.DrawSettingsSection -= DrawSettingsSection;
    }
    
    // Event handlers
    private void GUIChanged(object sender, EventArgs e)
    {
        OnGUIChanged();
    }

    private void UpdateSerializedObject(object sender, EventArgs e)
    {
        UpdateSerializedObject();
    }
    
    private void DrawSideBarSection(object sender, EventArgs e)
    {
        DrawSideBarSection();
    }
    
    private void DrawSettingsSection(object sender, EventArgs e)
    {
        DrawSettingsSection();
    }
    
    [MenuItem("Tools/Planet Generator")]
    public static void ShowWindow()
    {
        GetWindow<WindowView>(WindowTitle);
    }

    private void OnGUI()
    {
        _controller.OnUpdate();
        
        GUILayout.BeginHorizontal();
        _controller.OnDrawSideBarSection();
        _controller.OnDrawSettingsSection();
        GUILayout.EndHorizontal();
        
        _controller.OnGUIChanged();
    }

    #region EditorWindow Events
    
    private void UpdateSerializedObject()
    {
        _serializedObject.Update();
    }

    private void OnGUIChanged()
    {
        if (GUI.changed)
        {
            _serializedObject.ApplyModifiedProperties();
            UpdateSettingsSectionHeader();
        }
    }
    
    private void DrawSideBarSection()
    {
        var assetsInFolder = GetAllAssetNamesAndPaths();
        var areaRect = new Rect(0, 0, _sidebarFrameWidth, position.height);

        GUILayout.BeginVertical();
        GUILayout.BeginArea(areaRect);
        
        GUI.Box(areaRect, string.Empty);
        
        BeginDrawLeftScrollView();
        DrawSidebarHeader();
        DrawAssetButtonsInOrder(assetsInFolder);

        GUILayout.EndArea();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    
    private void DrawSettingsSection()
    {
        const float separationLine = 5f;
        var areaRect = new Rect(_sidebarFrameWidth + separationLine, 0, SetSettingsSectionWidth(), position.height);
        
        GUI.Box(areaRect, string.Empty);

        GUILayout.BeginVertical();
        GUILayout.BeginArea(areaRect);
        
        DrawSettingsHeader();
        DrawTabsAndCurrentTab();
        
        GUILayout.EndArea();
        GUILayout.EndVertical();
    }
    
    #endregion

    #region Sidebar Section

    private static void DrawSidebarHeader()
    {
        EditorGUILayout.Space(3);
        GUILayout.Label(SidebarHeader, SetCenteredBoldLabel());
        EditorGUILayout.Space(3);
    }

    private void BeginDrawLeftScrollView()
    {
        _leftScrollPosition = GUILayout.BeginScrollView(_leftScrollPosition, GUILayout.Width(_sidebarFrameWidth + 2));
    }

    private void DrawAssetButtonsInOrder((string name, string path)[] assetsInFolder)
    {
        var objectTypes = new[] { ObjectType.TerrestrialBody, ObjectType.SolidSphere };
        
        DrawSidebarSubHeader1();
        DrawAssetButtonsByType(assetsInFolder, objectTypes[1]);
        DrawSidebarSubHeader2();
        DrawAssetButtonsByType(assetsInFolder, objectTypes[0]);

    }

    private void DrawAssetButtonsByType((string name, string path)[] assetsInFolder, ObjectType objectTypes)
    {
        const float buttonBorderWith = 10f;
        var maxButtonWidth = MaxButtonWidth(assetsInFolder);
        var defaultStyle = SetButtonDefaultStyle(maxButtonWidth, buttonBorderWith);
        
        foreach (var asset in assetsInFolder)
        {
            var selectedAsset = SetSelectedAsset(asset);
            if (selectedAsset.ObjectType == objectTypes) continue;

            DrawAssetButton(selectedAsset, asset, defaultStyle);
        }
    }

    private static void DrawSidebarSubHeader2()
    {
        GUILayout.Label(SidebarSubHeader2, SetCenteredMiniLabel());
    }

    private static void DrawSidebarSubHeader1()
    {
        GUILayout.Label(SidebarSubHeader1, SetCenteredMiniLabel());
    }

    private GUIStyle SetButtonDefaultStyle(float maxButtonWidth, float buttonBorderWith)
    {
        var defaultStyle = new GUIStyle(GUI.skin.button);
        defaultStyle.normal.textColor = Color.white;
        defaultStyle.fixedWidth = maxButtonWidth + buttonBorderWith;

        // TODO: Find out why the background color is not applied.
        defaultStyle.normal.background = CreateColoredTexture(2, 2, Color.gray);
        defaultStyle.active.background = CreateColoredTexture(2, 2, Color.gray);
        return defaultStyle;
    }

    private float MaxButtonWidth((string name, string path)[] assetsInFolder)
    {
        const float frameBorderWidth = 15f;
        var maxButtonWidth = 0f;
        
        foreach (var asset in assetsInFolder)
        {
            var assetNameWidth = GUI.skin.button.CalcSize(new GUIContent(asset.name)).x;
            maxButtonWidth = Mathf.Max(maxButtonWidth, assetNameWidth);
            _sidebarFrameWidth = maxButtonWidth + frameBorderWidth;
        }

        return maxButtonWidth;
    }

    private void DrawAssetButton(ObjectSettings selectedAsset, (string name, string path) asset, GUIStyle buttonStyle)
    {
        if (GUILayout.Button(asset.name, buttonStyle))
        {
            if (selectedAsset == null)
            {
                Debug.LogWarning($"Something went wrong loading the asset from the path {asset.path}.");
                return;
            }

            _objectSettings = selectedAsset;
        }

        EditorGUILayout.Space(1);
    }

    private static ObjectSettings SetSelectedAsset((string name, string path) asset)
    {
        return AssetDatabase.LoadAssetAtPath<ObjectSettings>(asset.path);
    }
    
    private static Texture2D CreateColoredTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];
        for (int colorsIndex = 0; colorsIndex < pixels.Length; colorsIndex++)
            pixels[colorsIndex] = color;
        
        var texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    #endregion

    #region Settings Section
    
    private void DrawSettingsHeader()
    {
        EditorGUILayout.Space(3);
        GUILayout.Label(_settingsHeader, SetCenteredBoldLabel());
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
    }
    
    private void DrawTabsAndCurrentTab()
    {
        _tabIndex = GUILayout.Toolbar(_tabIndex, _tabHeaders);
        EditorGUILayout.Space(10);
        
        switch (_tabIndex)
        {
            case 0:
                DrawGeneralSettingsTab();
                break;
            case 1:
                DrawElevationSettingsTab();
                break;
        }
    }
    
    private void DrawGeneralSettingsTab()
    {
        DrawGeneralSettingsHeader();
        DrawObjectSettingsField();
        EditorGUILayout.PropertyField(_materialProperty);
        EditorGUILayout.PropertyField(_resolutionProperty);
        EditorGUILayout.PropertyField(_radiusProperty);
        if (_objectSettings.ObjectType == ObjectType.TerrestrialBody)
            EditorGUILayout.PropertyField(_gradientProperty);
    }

    private void DrawObjectSettingsField()
    {
        _objectSettings =
            (ObjectSettings)EditorGUILayout.ObjectField(_currentAssetLabel, _objectSettings, typeof(ObjectSettings), false);
    }

    private static void DrawGeneralSettingsHeader()
    {
        GUILayout.Label(GeneralSettingsHeader, SetCenteredBoldLabel());
    }

    private void DrawElevationSettingsTab()
    {
        DrawSurfaceSettingsHeader();
        DrawShapeTypeField();
        DrawElevationLayerSettings();
    }

    private void DrawElevationLayerSettings()
    {
        if (_objectSettings.ObjectType == ObjectType.TerrestrialBody)
        {
            DrawElevationLayerSettingsHeader();
            DrawNoiseLayerOptionButtons();
            DrawRightScrollView();
            DrawNoiseLayer();
            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("NoiseLayers are not available for Solid Spheres.", MessageType.Info);
        }
    }

    private void DrawRightScrollView()
    {
        _rightScrollPosition = GUILayout.BeginScrollView(_rightScrollPosition, GUILayout.Width(SetSettingsSectionWidth()));
    }

    private static void DrawElevationLayerSettingsHeader()
    {
        EditorGUILayout.Space(3);
        GUILayout.Label(ElevationLayerSettingsHeader, SetCenteredMiniBoldLabel());
    }

    private void DrawShapeTypeField()
    {
        EditorGUIUtility.labelWidth = SetSettingsSectionWidth() * 0.4f;
        EditorGUILayout.PropertyField(_objectTypeProperty, new GUIContent(_objectTypeLabel));
        EditorGUIUtility.labelWidth = 0f;
        EditorGUILayout.Space(5);
    }

    private static void DrawSurfaceSettingsHeader()
    {
        GUILayout.Label(SurfaceSettingsHeader, SetCenteredBoldLabel());
        EditorGUILayout.Space(1);
    }

    private void DrawNoiseLayerOptionButtons()
    {
        var buttonWidth = SetSettingsSectionWidth() * 0.5f - 10;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(_addLayerButton, GUILayout.Width(buttonWidth)))
            AddNoiseLayer();
        
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button(_removeLayerButton, GUILayout.Width(buttonWidth)))
            RemoveLastNoiseLayer();

        GUILayout.EndHorizontal();
    }
    
    public void DrawSaveButton()
    {
        if (GUILayout.Button(_updateButton))
        {
            EditorUtility.SetDirty(_objectSettings);
            AssetDatabase.SaveAssets();
        }
    }

    private void AddNoiseLayer()
    {
        if (_objectSettings.NoiseLayers.Length >= 3)
        {
            Debug.LogWarning("Cannot add more than 3 NoiseLayers.");
            return;
        }

        // Create a new array with the length of the current array + 1.
        var currentLayers = _objectSettings.NoiseLayers;
        var newLayers = new NoiseLayer[currentLayers.Length + 1];

        // Copy the current array to the new array.
        currentLayers.CopyTo(newLayers, 0);
        newLayers[currentLayers.Length] = new NoiseLayer();

        UpdateNoiseLayerArray(newLayers);
    }

    private void RemoveLastNoiseLayer()
    {
        if (_objectSettings.NoiseLayers.Length <= 0)
        {
            Debug.LogWarning("Cannot remove NoiseLayer from empty array.");
            return;
        }

        var currentLayers = _objectSettings.NoiseLayers;
        if (currentLayers.Length <= 0) return;
        
        // Create a new array with the length of the current array - 1.
        var newLayers = new NoiseLayer[currentLayers.Length - 1];
   
        // Copy the new array to the current array.
        for (int layerIndex = 0; layerIndex < newLayers.Length; layerIndex++)
            newLayers[layerIndex] = currentLayers[layerIndex];

        UpdateNoiseLayerArray(newLayers);
    }

    private void UpdateNoiseLayerArray(NoiseLayer[] newLayers)
    {
        // Set the new array to current array.
        _objectSettings.NoiseLayers = newLayers;

        // Update the serialized object after modifying the array(prevents OOB exception).
        _serializedObject = new SerializedObject(_objectSettings);
    }

    private void DrawNoiseLayer()
    {
        if (_objectSettings.NoiseLayers == null) return;
        
        for (int layerIndex = 0; layerIndex < _objectSettings.NoiseLayers.Length; layerIndex++)
        {
            if (layerIndex < 0 || layerIndex >= _objectSettings.NoiseLayers.Length) return;
            
            var noiseLayerRect = CalculateNoiseLayerLayout();
            
            GetNoiseLayerProperty(layerIndex);
            DrawNoiseLayerFoldout(layerIndex);

            if (_noiseLayerProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
            
                GetNoiseLayerProperties();
                DrawNoiseLayerProperties(noiseLayerRect);

                EditorGUI.indentLevel--;
            }
        }
        
        EditorGUIUtility.labelWidth = 0f;
    }

    private void GetNoiseLayerProperty(int layerIndex)
    {
        _noiseLayerProperty = _serializedObject.FindProperty("NoiseLayers").GetArrayElementAtIndex(layerIndex);
    }

    private void DrawNoiseLayerFoldout(int layerIndex)
    {
        var layerName = $"Elevation Layer ({layerIndex + 1})";
        EditorGUILayout.Space(5);
        _noiseLayerProperty.isExpanded =
            EditorGUILayout.Foldout(_noiseLayerProperty.isExpanded, new GUIContent(layerName));
    }

    private void DrawNoiseLayerProperties(Rect noiseLayerRect)
    {
        noiseLayerRect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(noiseLayerRect, _enabledProperty);

        // Draw the UseFirstLayerAsMask property.
        noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(noiseLayerRect, _useFirstLayerAsMaskProperty);

        // Draw the NoiseSettings property.
        noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(noiseLayerRect, _noiseSettingsProperty, true);
    }

    private Rect CalculateNoiseLayerLayout()
    {
        // Calculate the height of the NoiseLayer with all its properties.
        var noiseLayerHeight = EditorGUIUtility.singleLineHeight;
        noiseLayerHeight += EditorGUI.GetPropertyHeight(_noiseLayerProperty, true);

        // Set the label width to 40% of the settings section width.
        EditorGUIUtility.labelWidth = SetSettingsSectionWidth() * 0.4f;
        return EditorGUILayout.GetControlRect(false, noiseLayerHeight);
    }

    private void GetNoiseLayerProperties()
    {
        _enabledProperty = _noiseLayerProperty.FindPropertyRelative("Enabled");
        _useFirstLayerAsMaskProperty = _noiseLayerProperty.FindPropertyRelative("UseFirstLayerAsMask");
        _noiseSettingsProperty = _noiseLayerProperty.FindPropertyRelative("NoiseSettings");
    }

    /// <summary>
    /// Tuple with asset name and asset path. Less than three.
    /// </summary>
    /// <returns>Returns a tuple array with all asset. Each tuple contains an asset name and path</returns>
    private static (string name, string path)[] GetAllAssetNamesAndPaths()
    {
        // Get all asset paths from the folder.
        const string folderPath = FolderPath.RootInstances;
        var assetPaths = AssetDatabase.FindAssets("t:ObjectSettings", new[] { folderPath });

        // Create a tuple array with the asset name and path.
        var assetNames = new (string name, string path)[assetPaths.Length];
        
        // Fill the tuple array with the asset name and path.
        for (var assetIndex = 0; assetIndex < assetPaths.Length; assetIndex++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[assetIndex]);
            var assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            assetNames[assetIndex] = (assetName, assetPath);
        }

        return assetNames;
    }
    
    #endregion
    
    private void UpdateSettingsSectionHeader()
    {
        _settingsHeader = $"Settings [{_objectSettings.name}]";
    }

    private float SetSettingsSectionWidth()
    {
        return position.width - _sidebarFrameWidth - 10;
    }

    private static GUIStyle SetCenteredBoldLabel()
    {
        return new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
    }
    
    private static GUIStyle SetCenteredMiniBoldLabel()
    {
        return new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleCenter };
    }
    
    private static GUIStyle SetCenteredMiniLabel()
    {
        return new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
    }
}