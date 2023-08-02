# if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class DrawNoiseLayers
    {
        private readonly WindowLayout _layout;

        public DrawNoiseLayers(WindowLayout layout)
        {
            _layout = layout;
        }

        public void DrawSurfaceSettingsHeader()
        {
            GUILayout.Label(TextHolder.SurfaceSettingsHeader, LabelStyle.SetCenteredBoldLabel());
            EditorGUILayout.Space(1);
        }

        public void DrawNoiseLayer()
        {
            if (_layout.ObjectSettings.NoiseLayers == null) return;
        
            for (int layerIndex = 0; layerIndex < _layout.ObjectSettings.NoiseLayers.Length; layerIndex++)
            {
                if (layerIndex < 0 || layerIndex >= _layout.ObjectSettings.NoiseLayers.Length) return;
            
                _layout.SetNoiseLayerProperty(layerIndex);
                DrawNoiseLayerFoldout(layerIndex);

                if (_layout.NoiseLayerProperty.isExpanded)
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

        private void DrawNoiseLayerFoldout(int layerIndex)
        {
            var layerName = $"Elevation Layer ({layerIndex + 1})";
            EditorGUILayout.Space(5);
            _layout.NoiseLayerProperty.isExpanded =
                EditorGUILayout.Foldout(_layout.NoiseLayerProperty.isExpanded, new GUIContent(layerName));
        }

        private void DrawNoiseLayerProperties(Rect noiseLayerRect)
        {
            noiseLayerRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(noiseLayerRect, _layout.EnabledProperty);

            // Draw the UseFirstLayerAsMask property.
            noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(noiseLayerRect, _layout.UseFirstLayerAsMaskProperty);

            // Draw the NoiseSettings property.
            noiseLayerRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(noiseLayerRect, _layout.NoiseSettingsProperty, true);
        }

        private Rect CalculateNoiseLayerLayout()
        {
            // Calculate the height of the NoiseLayer with all its properties.
            var noiseLayerHeight = EditorGUIUtility.singleLineHeight;
            noiseLayerHeight += EditorGUI.GetPropertyHeight(_layout.NoiseLayerProperty, true);

            // Set the label width to 40% of the settings section width.
            EditorGUIUtility.labelWidth = _layout.SetSettingsSectionWidth() * 0.4f;
            return EditorGUILayout.GetControlRect(false, noiseLayerHeight);
        }

        private void GetNoiseLayerProperties()
        {
            _layout.EnabledProperty = _layout.NoiseLayerProperty.FindPropertyRelative("Enabled");
            _layout.UseFirstLayerAsMaskProperty = _layout.NoiseLayerProperty.FindPropertyRelative("UseFirstLayerAsMask");
            _layout.NoiseSettingsProperty = _layout.NoiseLayerProperty.FindPropertyRelative("NoiseSettings");
        }
    }
}

#endif

