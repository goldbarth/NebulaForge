# if UNITY_EDITOR

using PlanetSettings;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class GeneralTab
    {
        private readonly WindowLayout _layout;

        public GeneralTab(WindowLayout layout)
        {
            _layout = layout;
        }

        public void DrawGeneralSettingsTab()
        {
            DrawGeneralSettingsHeader();
            GUILayout.Space(5);
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
            _layout.ObjectSettings =
                (ObjectSettings)EditorGUILayout.ObjectField(TextHolder.CurrentAssetLabel, _layout.ObjectSettings, typeof(ObjectSettings), false);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPropertyFields()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_layout.MaterialProperty);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_layout.ResolutionProperty);
            EditorGUILayout.PropertyField(_layout.RadiusProperty);
            if (_layout.ObjectSettings.ObjectType == ObjectType.TerrestrialBody)
                EditorGUILayout.PropertyField(_layout.GradientProperty);
        }
    }
}

#endif

