using EditorWindowDependencies;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class SurfaceTab
    {
        private readonly DrawNoiseLayerOptions _drawNoiseLayerOptions;
        private readonly DrawNoiseLayers _drawNoiseLayers;
        private readonly WindowView _view;

        private Vector2 _rightScrollPosition;

        public SurfaceTab(WindowView view)
        {
            _view = view;
            _drawNoiseLayers = new DrawNoiseLayers(_view);
            _drawNoiseLayerOptions = new DrawNoiseLayerOptions(_view);
        }

        public void DrawElevationSettingsTab()
        {
            _drawNoiseLayers.DrawSurfaceSettingsHeader();
            DrawShapeTypeField();
            DrawElevationLayerSettings();
        }

        private void DrawElevationLayerSettings()
        {
            if (_view.ObjectSettings.ObjectType == ObjectType.TerrestrialBody)
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
            _rightScrollPosition = GUILayout.BeginScrollView(_rightScrollPosition, GUILayout.Width(_view.SetSettingsSectionWidth()));
        }

        private static void DrawElevationLayerSettingsHeader()
        {
            EditorGUILayout.Space(3);
            GUILayout.Label(TextHolder.ElevationLayerSettingsHeader, LabelStyle.SetCenteredMiniBoldLabel());
        }

        private void DrawShapeTypeField()
        {
            EditorGUIUtility.labelWidth = _view.SetSettingsSectionWidth() * 0.4f;
            EditorGUILayout.PropertyField(_view.ObjectTypeProperty, new GUIContent(TextHolder.ObjectTypeLabel));
            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.Space(5);
        }
    }
}

