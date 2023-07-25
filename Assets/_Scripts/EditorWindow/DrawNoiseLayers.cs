using UnityEditor;
using UnityEngine;

public class DrawNoiseLayers
{
    private readonly WindowView _view;

    public DrawNoiseLayers(WindowView view)
    {
        _view = view;
    }

    public void DrawSurfaceSettingsHeader()
    {
        GUILayout.Label(TextHolder.SurfaceSettingsHeader, LabelStyle.SetCenteredBoldLabel());
        EditorGUILayout.Space(1);
    }

    public void DrawNoiseLayer()
    {
        if (_view.ObjectSettings.NoiseLayers == null) return;
        
        for (int layerIndex = 0; layerIndex < _view.ObjectSettings.NoiseLayers.Length; layerIndex++)
        {
            if (layerIndex < 0 || layerIndex >= _view.ObjectSettings.NoiseLayers.Length) return;
            
            GetNoiseLayerProperty(layerIndex);
            DrawNoiseLayerFoldout(layerIndex);

            if (_view.NoiseLayerProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
            
                var noiseLayerRect = CalculateNoiseLayerLayout();
                GetNoiseLayerProperties();
                DrawNoiseLayerProperties(noiseLayerRect);

                EditorGUI.indentLevel--;
            }
        }
        
        EditorGUIUtility.labelWidth = 0f;
    }

    private void GetNoiseLayerProperty(int layerIndex)
    {
        _view.NoiseLayerProperty = _view.SerializedObject.FindProperty("NoiseLayers").GetArrayElementAtIndex(layerIndex);
    }

    private void DrawNoiseLayerFoldout(int layerIndex)
    {
        var layerName = $"Elevation Layer ({layerIndex + 1})";
        EditorGUILayout.Space(5);
        _view.NoiseLayerProperty.isExpanded =
            EditorGUILayout.Foldout(_view.NoiseLayerProperty.isExpanded, new GUIContent(layerName));
    }

    private void DrawNoiseLayerProperties(Rect noiseLayerRect)
    {
        noiseLayerRect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(noiseLayerRect, _view.EnabledProperty);

        // Draw the UseFirstLayerAsMask property.
        noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(noiseLayerRect, _view.UseFirstLayerAsMaskProperty);

        // Draw the NoiseSettings property.
        noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(noiseLayerRect, _view.NoiseSettingsProperty, true);
    }

    private Rect CalculateNoiseLayerLayout()
    {
        // Calculate the height of the NoiseLayer with all its properties.
        var noiseLayerHeight = EditorGUIUtility.singleLineHeight;
        noiseLayerHeight += EditorGUI.GetPropertyHeight(_view.NoiseLayerProperty, true);

        // Set the label width to 40% of the settings section width.
        EditorGUIUtility.labelWidth = _view.SetSettingsSectionWidth() * 0.4f;
        return EditorGUILayout.GetControlRect(false, noiseLayerHeight);
    }

    private void GetNoiseLayerProperties()
    {
        _view.EnabledProperty = _view.NoiseLayerProperty.FindPropertyRelative("Enabled");
        _view.UseFirstLayerAsMaskProperty = _view.NoiseLayerProperty.FindPropertyRelative("UseFirstLayerAsMask");
        _view.NoiseSettingsProperty = _view.NoiseLayerProperty.FindPropertyRelative("NoiseSettings");
    }
}