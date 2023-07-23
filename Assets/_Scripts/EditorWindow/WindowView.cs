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
    
    private readonly string[] _tabHeaders = { "Settings", "Surface Settings" };
    private Vector2 _scrollPosition;
    private int _tabIndex;

    private void OnEnable()
    {
        _object = FindObjectOfType<ObjectGenerator>();
        _objectSettings = _object.ObjectSettings;

        _serializedObject = new SerializedObject(_objectSettings);
        _objectTypeProperty = _serializedObject.FindProperty("ObjectType");
        _materialProperty = _serializedObject.FindProperty("Material");
        _resolutionProperty = _serializedObject.FindProperty("Resolution");
        _radiusProperty = _serializedObject.FindProperty("Radius");
        _gradientProperty = _serializedObject.FindProperty("Gradient");
        _noiseLayerProperty  = _serializedObject.FindProperty("NoiseLayers");
        
        _controller = new WindowController(this);
    }
    
    [MenuItem("Tools/Planet Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<WindowView>(nameof(WindowView));
        window.titleContent = new GUIContent("Planet Generator");
    }

    private void OnGUI()
    {
        _serializedObject.Update();
        
        var centeredBoldLabel = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        
        GUILayout.BeginHorizontal();

        // Left side of the split editor window (asset selection)
        GUILayout.BeginVertical();
        
        GUILayout.Label("Objects", centeredBoldLabel);
        DrawSideBarButtons();

        GUILayout.EndVertical();

        // Right side of the split editor window (tabs and settings)
        GUILayout.BeginVertical();
        
        EditorGUILayout.Space(3);
        GUILayout.Label($"{_objectSettings.name}", centeredBoldLabel);
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

        DrawTabsAndCurrentTab();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        
        
        _controller.OnGUIChanged();
    }

    private void DrawTabsAndCurrentTab()
    {
        _tabHeaders[0] = "General";
        _tabHeaders[1] = "Elevation Surface";
        _tabIndex = GUILayout.Toolbar(_tabIndex, _tabHeaders);
        
        EditorGUILayout.Space(10);

        switch (_tabIndex)
        {
            case 0:
                DrawGeneralSettingsTab();
                break;
            case 1:
                DrawElevationSurfaceSettingsTab();
                break;
        }
    }

    private void DrawSideBarButtons()
    {
        var assetsInFolder = GetAllAssetNamesAndPaths();
        
        // Calculate the maximum button and frame width based on the asset names.
        var frameWidth = 0f;
        var maxButtonWidth = 0f;
        var frameBorderWidth = 15f;
        foreach (var asset in assetsInFolder)
        {
            var assetWidth = GUI.skin.button.CalcSize(new GUIContent(asset.name)).x;
            maxButtonWidth = Mathf.Max(maxButtonWidth, assetWidth);
            frameWidth = maxButtonWidth + frameBorderWidth;
        }
        
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.fixedWidth = maxButtonWidth + 10f;
        buttonStyle.normal.background =
            CreateTexture(2, 2, new Color(0.2f, 0.5f, 0.8f));

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(frameWidth));
        
        // Draw a frame around all buttons
        var buttonSpacing = 7f;
        GUI.Box(new Rect(0, 0, frameWidth, assetsInFolder.Length * (EditorGUIUtility.singleLineHeight + buttonSpacing)), string.Empty);
        
        foreach (var asset in assetsInFolder)
        {
            if (GUILayout.Button(asset.name, buttonStyle))
            {
                var selectedAsset = AssetDatabase.LoadAssetAtPath<ObjectSettings>(asset.path);
                if (selectedAsset == null)
                {
                    Debug.LogWarning($"Something went wrong loading the asset from the path {asset.path}.");
                    return;
                }

                _objectSettings = selectedAsset;
                _object.GeneratePlanet();
            }

            EditorGUILayout.Space(1);
            
        }
            
        GUILayout.EndScrollView();
    }

    // Method to create a colored texture for the button background.
    private Texture2D CreateTexture(int width, int height, Color color)
    {
        var colors = new Color[width * height];
        for (int colorsIndex = 0; colorsIndex < colors.Length; colorsIndex++)
            
            colors[colorsIndex] = color;
        
        var result = new Texture2D(width, height);
        result.SetPixels(colors);
        result.Apply();
        return result;
    }
    
    private void DrawGeneralSettingsTab()
    {
        _objectSettings = (ObjectSettings)EditorGUILayout.ObjectField("Planet Asset", _objectSettings, typeof(ObjectSettings), false);
        EditorGUILayout.PropertyField(_materialProperty);
        EditorGUILayout.PropertyField(_resolutionProperty);
        EditorGUILayout.PropertyField(_radiusProperty);
        if (_objectSettings.ObjectType == ObjectType.TerrestrialBody)
            EditorGUILayout.PropertyField(_gradientProperty);
    }

    private void DrawElevationSurfaceSettingsTab()
    {
        var centeredBoldLabel = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        var centeredMiniBoldLabel = new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleCenter };
        
        GUILayout.Label("Elevation Surface", centeredBoldLabel);
        EditorGUILayout.Space(1);
        EditorGUILayout.PropertyField(_objectTypeProperty, new GUIContent("Object Type"));
        EditorGUILayout.Space(5);

        // TODO: Scrollbar for the NoiseLayers
        if (_objectSettings.ObjectType == ObjectType.TerrestrialBody)
        {
            DrawNoiseLayerOptionButtons();
            EditorGUILayout.Space(3);
            GUILayout.Label("Elevation Layer Settings", centeredMiniBoldLabel);
            DrawNoiseLayer();
        }
        else
        {
            EditorGUILayout.HelpBox("NoiseLayers are not available for Solid Spheres.", MessageType.Info);
        }
    }

    public void OnGUIChanged()
    {
        if (GUI.changed)
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }

    public void DrawNoiseLayerOptionButtons()
    {
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Layer"))
            _controller.AddNoiseLayer();
        
        if (GUILayout.Button("Remove Layer"))
            _controller.RemoveLastNoiseLayer();

        GUILayout.EndHorizontal();
    }
    
    public void DrawSaveButton()
    {
        if (GUILayout.Button("Update"))
        {
            EditorUtility.SetDirty(_objectSettings);
            AssetDatabase.SaveAssets();
        }
    }

    public void AddNoiseLayer()
    {
        if (_objectSettings.NoiseLayers.Length >= 3)
        {
            Debug.LogWarning("Cannot add more than 3 NoiseLayers.");
            return;
        }

        var currentLayers = _objectSettings.NoiseLayers;
        var newLayers = new NoiseLayer[currentLayers.Length + 1];

        currentLayers.CopyTo(newLayers, 0);
        newLayers[currentLayers.Length] = new NoiseLayer();

        _objectSettings.NoiseLayers = newLayers;

        // Update the serialized object after modifying the array(prevents OOB exception)
        _serializedObject = new SerializedObject(_objectSettings);
    }

    public void RemoveLastNoiseLayer()
    {
        if (_objectSettings.NoiseLayers.Length <= 0)
        {
            Debug.LogWarning("Cannot remove NoiseLayer from empty array.");
            return;
        }

        var currentLayers = _objectSettings.NoiseLayers;
        if (currentLayers.Length <= 0) return;
        
        var newLayers = new NoiseLayer[currentLayers.Length - 1];
        for (int i = 0; i < newLayers.Length; i++)
            newLayers[i] = currentLayers[i];

        _objectSettings.NoiseLayers = newLayers;
    }

    public void DrawNoiseLayer()
    {
        if (_objectSettings.NoiseLayers == null) return;
        
        for (int layerIndex = 0; layerIndex < _objectSettings.NoiseLayers.Length; layerIndex++)
        {
            if (layerIndex < 0 || layerIndex >= _objectSettings.NoiseLayers.Length)
                return;

            // Get the NoiseLayer property
            _noiseLayerProperty = _serializedObject.FindProperty("NoiseLayers").GetArrayElementAtIndex(layerIndex);
            var customName = $"Elevation Layer {layerIndex + 1}";

            // Draw the foldout
            _noiseLayerProperty.isExpanded =
                EditorGUILayout.Foldout(_noiseLayerProperty.isExpanded, new GUIContent(customName));

            // Draw the NoiseLayer fields if the foldout is open
            if (!_noiseLayerProperty.isExpanded) continue;
            
            EditorGUI.indentLevel++;
            
            // Draw individual properties of NoiseLayer
            _enabledProperty = _noiseLayerProperty.FindPropertyRelative("Enabled");
            _useFirstLayerAsMaskProperty = _noiseLayerProperty.FindPropertyRelative("UseFirstLayerAsMask");
            _noiseSettingsProperty = _noiseLayerProperty.FindPropertyRelative("NoiseSettings");
            
            // Calculate the height of the NoiseLayer with all its properties
            var noiseLayerHeight = EditorGUIUtility.singleLineHeight;
            //noiseLayerHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            noiseLayerHeight += EditorGUI.GetPropertyHeight(_noiseLayerProperty, true);

            // Draw the Enabled property
            var noiseLayerRect = EditorGUILayout.GetControlRect(false, noiseLayerHeight);
            noiseLayerRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(noiseLayerRect, _enabledProperty);

            // Draw the UseFirstLayerAsMask property
            noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(noiseLayerRect, _useFirstLayerAsMaskProperty);

            // Draw the NoiseSettings property
            noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(noiseLayerRect, _noiseSettingsProperty, true);

            EditorGUI.indentLevel--;
        }
    }
    
    /// <summary>
    /// Tuple of asset name and asset path. <3
    /// </summary>
    /// <returns>Returns a tuple array with all asset. Each tuple contains a asset name and path</returns>
    private (string name, string path)[] GetAllAssetNamesAndPaths()
    {
        // Fetch all the asset paths in the folder we want to choose assets from.
        const string folderPath = FolderPath.RootInstances;
        var assetPaths = AssetDatabase.FindAssets("t:ObjectSettings", new[] { folderPath });

        // Convert asset paths to asset names.
        var assetNames = new (string name, string path)[assetPaths.Length];
        for (var assetIndex = 0; assetIndex < assetPaths.Length; assetIndex++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[assetIndex]);
            var assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            assetNames[assetIndex] = (assetName, assetPath);
        }

        return assetNames;
    }
}