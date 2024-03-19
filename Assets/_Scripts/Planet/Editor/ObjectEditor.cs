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
        
        private readonly string _generatorOptionsLabel = "Generator Options";
        private readonly string _generateObjectButton = "Generate Object";
        private readonly string _removeObjectButton = "Remove Object";
        private readonly string _editorWindowLabel = "Editor Window";
        private readonly string _openButton = "Open";

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
        
            EditorGUILayout.LabelField(_generatorOptionsLabel, LabelStyle.SetCenteredBoldLabel());
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
            EditorGUILayout.LabelField(_editorWindowLabel, LabelStyle.SetCenteredBoldLabel());
            GUILayout.Space(5);
        
            EditorGUILayout.BeginHorizontal();
            DrawOpenEditorWindowButton();
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.EndVertical();
        
            GUILayout.Space(5);
        }

        private void DrawOpenEditorWindowButton()
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_openButton,
                    LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(_openButton), ButtonBorderWidth)))
                ObjectGeneratorWindow.ShowWindow();
            GUILayout.FlexibleSpace();
        }

        private void DrawRemovePlanetButton()
        {
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_removeObjectButton,
                    LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(_removeObjectButton), ButtonBorderWidth)))
                _object.RemovePlanet();
            GUILayout.FlexibleSpace();
        }

        private void DrawGeneratePlanetButton()
        {
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_generateObjectButton,
                    LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(_generateObjectButton), ButtonBorderWidth)))
            {
                _object.GenerateObject();
                _object.UpdateGradientSettings();
            }

            GUILayout.FlexibleSpace();
        }
    }
}

#endif
