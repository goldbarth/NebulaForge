# if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class SurfaceTab
    {
        private readonly DrawNoiseLayerOptions _drawNoiseLayerOptions;
        private readonly DrawNoiseLayers _drawNoiseLayers;
        private readonly ObjectGeneratorWindow _layout;

        private Vector2 _rightScrollPosition;

        public SurfaceTab(ObjectGeneratorWindow layout)
        {
            _layout = layout;
            _drawNoiseLayers = new DrawNoiseLayers(_layout);
            _drawNoiseLayerOptions = new DrawNoiseLayerOptions(_layout);
        }

        public void DrawElevationSettingsTab()
        {
            _drawNoiseLayers.DrawSurfaceSettingsHeader();
            DrawShapeTypeField();
            DrawElevationLayerSettings();
        }

        private void DrawElevationLayerSettings()
        {
            if (_layout.ObjectSettings.ObjectType == ObjectType.TerrestrialBody)
            {
                DrawElevationLayerSettingsHeader();
                _drawNoiseLayerOptions.DrawNoiseLayerOptionButtons();
                DrawRightScrollView();
                _drawNoiseLayers.DrawNoiseLayer();
                GUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("NoiseLayers are not available for Solid Spheres.", MessageType.Info);
            }
        }

        private void DrawRightScrollView()
        {
            _rightScrollPosition = GUILayout.BeginScrollView(_rightScrollPosition, GUILayout.Width(_layout.SetSettingsSectionWidth()));
        }

        private static void DrawElevationLayerSettingsHeader()
        {
            EditorGUILayout.Space(3);
            GUILayout.Label(TextHolder.ElevationLayerSettingsHeader, LabelStyle.SetCenteredMiniBoldLabel());
        }

        private void DrawShapeTypeField()
        {
            EditorGUIUtility.labelWidth = _layout.SetSettingsSectionWidth() * 0.4f;
            EditorGUILayout.PropertyField(_layout.ObjectTypeProperty, new GUIContent(TextHolder.ObjectTypeLabel));
            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.Space(5);
        }
    }
}

#endif

