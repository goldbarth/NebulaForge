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
    
    private SerializedProperty _noiseLayersProperty;
    private SerializedProperty _enabledProperty;
    private SerializedProperty _useFirstLayerAsMaskProperty;
    private SerializedProperty _noiseSettingsProperty;
    
    private Vector2 _scrollPosition;

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
        _noiseLayersProperty  = _serializedObject.FindProperty("NoiseLayers");
        
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
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        _serializedObject.Update();

        _controller.DrawProperties();
        
        if (_objectSettings.ObjectType == ObjectType.TerrestrialBody)
        {
            _controller.DrawButtons();
            _controller.DrawNoiseLayer();
        }

        EditorGUILayout.EndScrollView();
            _controller.OnGUIChanged();
    }

    public void OnGUIChanged()
    {
        if (GUI.changed)
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }

    public void DrawButtons()
    {
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Noise Layer"))
            _controller.AddNoiseLayer();
        
        if (GUILayout.Button("Remove Last Noise Layer"))
            _controller.RemoveLastNoiseLayer();

        if (GUILayout.Button("Save Asset"))
        {
            // TODO: save the asset only if the user clicks on the save button
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(_objectSettings);
        }

        GUILayout.EndHorizontal();
    }

    public void DrawProperties()
    {
        _objectSettings = (ObjectSettings)EditorGUILayout.ObjectField("Object Settings", _objectSettings, typeof(ObjectSettings), false);
        EditorGUILayout.PropertyField(_objectTypeProperty, true);
        EditorGUILayout.PropertyField(_materialProperty, true);
        EditorGUILayout.PropertyField(_resolutionProperty, true);
        EditorGUILayout.PropertyField(_radiusProperty, true);
        if (_objectSettings.ObjectType == ObjectType.TerrestrialBody)
            EditorGUILayout.PropertyField(_gradientProperty, true);
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
            _noiseLayersProperty = _serializedObject.FindProperty("NoiseLayers").GetArrayElementAtIndex(layerIndex);
            var customName = $"Elevation Layer {layerIndex + 1}";

            // Draw the foldout
            _noiseLayersProperty.isExpanded =
                EditorGUILayout.Foldout(_noiseLayersProperty.isExpanded, new GUIContent(customName));

            // Draw the NoiseLayer fields if the foldout is open
            if (!_noiseLayersProperty.isExpanded) continue;
            
            EditorGUI.indentLevel++;
            
            // Draw individual properties of NoiseLayer
            _enabledProperty = _noiseLayersProperty.FindPropertyRelative("Enabled");
            _useFirstLayerAsMaskProperty = _noiseLayersProperty.FindPropertyRelative("UseFirstLayerAsMask");
            _noiseSettingsProperty = _noiseLayersProperty.FindPropertyRelative("NoiseSettings");
            
            // Calculate the height of the NoiseLayer with all its properties
            var noiseLayerHeight = EditorGUIUtility.singleLineHeight;
            noiseLayerHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            noiseLayerHeight += EditorGUI.GetPropertyHeight(_noiseLayersProperty, true);

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
}