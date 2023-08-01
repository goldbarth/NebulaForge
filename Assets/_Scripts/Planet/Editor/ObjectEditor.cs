#if UNITY_EDITOR

using CustomEditorWindow.Dependencies;
using CustomEditorWindow;
using UnityEditor;
using UnityEngine;

namespace Planet
{
    [CustomEditor(typeof(ObjectGenerator))]
    public class ObjectEditor : Editor
    {
        private const float ButtonBorderWidth = 15f;
    
        private ObjectGenerator _object;

        private void OnEnable()
        {
            _object = (ObjectGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            using var check = new EditorGUI.ChangeCheckScope();

            DrawButtons();
        
            OnChanged(check);
        }

        private void OnChanged(EditorGUI.ChangeCheckScope check)
        {
            if (check.changed)
            {
                _object.GenerateObject();
            }
        }

        private void DrawButtons()
        {
            GUILayout.Space(7);
            EditorGUILayout.BeginVertical();
        
            EditorGUILayout.LabelField("Generator Options", LabelStyle.SetCenteredBoldLabel());
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            DrawGeneratePlanetButton();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
        
            EditorGUILayout.BeginHorizontal();
            DrawRemovePlanetButton();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Editor Window", LabelStyle.SetCenteredBoldLabel());
            GUILayout.Space(5);
        
            EditorGUILayout.BeginHorizontal();
            DrawOpenEditorWindowButton();
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.EndVertical();
        
            GUILayout.Space(5);
        }

        private void DrawOpenEditorWindowButton()
        {
            var buttonName = "Open";
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonName,
                    LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(buttonName), ButtonBorderWidth)))
                WindowView.ShowWindow();
            GUILayout.FlexibleSpace();
        }

        private void DrawRemovePlanetButton()
        {
            var buttonName = "Remove Object";
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonName,
                    LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(buttonName), ButtonBorderWidth)))
                _object.RemovePlanet();
            GUILayout.FlexibleSpace();
        }

        private void DrawGeneratePlanetButton()
        {
            var buttonName = "Generate Object";
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonName,
                    LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(buttonName), ButtonBorderWidth)))
            {
                _object.GenerateObject();
                _object.OnColorSettingsUpdated();
            }

            GUILayout.FlexibleSpace();
        }
    }
}

#endif
