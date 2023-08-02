using EditorWindowDependencies;
using PlanetSettings;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class GeneralTab
    {
        private readonly WindowView _view;

        public GeneralTab(WindowView view)
        {
            _view = view;
        }

        public void DrawGeneralSettingsTab()
        {
            DrawGeneralSettingsHeader();
            DrawObjectSettingsField();
            DrawPropertyFields();
        }

        private void DrawGeneralSettingsHeader()
        {
            GUILayout.Label(TextHolder.GeneralSettingsHeader, LabelStyle.SetCenteredBoldLabel());
        }

        private void DrawObjectSettingsField()
        {
            EditorGUI.BeginDisabledGroup(true);
            _view.ObjectSettings =
                (ObjectSettings)EditorGUILayout.ObjectField(TextHolder.CurrentAssetLabel, _view.ObjectSettings, typeof(ObjectSettings), false);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPropertyFields()
        {
            EditorGUILayout.PropertyField(_view.MaterialProperty);
            EditorGUILayout.PropertyField(_view.ResolutionProperty);
            EditorGUILayout.PropertyField(_view.RadiusProperty);
            if (_view.ObjectSettings.ObjectType == ObjectType.TerrestrialBody)
                EditorGUILayout.PropertyField(_view.GradientProperty);
        }
    }
}

