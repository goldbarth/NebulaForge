using CustomEditorWindow;
using EditorWindowDependencies;
using UnityEditor;
using UnityEngine;

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

        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Current Asset", LabelStyle.SetCenteredBoldLabel());
        
        // Center-align the ObjectField label
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        
        _object.ObjectSettings = EditorGUILayout.ObjectField("Object Settings", _object.ObjectSettings, typeof(ObjectSettings), true) as ObjectSettings;
        
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
                LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(buttonName), buttonBorderWidth)))
            WindowView.ShowWindow();
        GUILayout.FlexibleSpace();
    }
}