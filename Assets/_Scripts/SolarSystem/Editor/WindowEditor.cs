#if UNITY_EDITOR

using CustomEditorWindow.Dependencies;
using CustomEditorWindow;
using PlanetSettings;
using UnityEditor;
using UnityEngine;
using Planet;

namespace SolarSystem
{
    [CustomEditor(typeof(CelestialObject))]
    public class WindowEditor : Editor
    {
        private ObjectGenerator _object;

        private void OnEnable()
        {
            _object = ((CelestialObject)target).GetComponentInChildren<ObjectGenerator>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(_object == null) return;
            
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Current Asset", LabelStyle.SetCenteredBoldLabel());

            EditorGUI.BeginDisabledGroup(true);
            _object.ObjectSettings = EditorGUILayout.ObjectField("Object Settings", _object.ObjectSettings, typeof(ObjectSettings), true) as ObjectSettings;
            EditorGUI.EndDisabledGroup();
        
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Object Editor Window", LabelStyle.SetCenteredBoldLabel());
            GUILayout.Space(5);
        
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            DrawOpenEditorWindowButton();
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.EndVertical();
        }
    
        private void DrawOpenEditorWindowButton()
        {
            const float buttonBorderWidth = 15f;
            var buttonName = "Open";
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonName,
                    LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(buttonName), buttonBorderWidth)))
                ObjectGeneratorWindow.ShowWindow();
            GUILayout.FlexibleSpace();
        }
    }
}

#endif