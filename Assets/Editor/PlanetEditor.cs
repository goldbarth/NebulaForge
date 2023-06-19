using Object = UnityEngine.Object;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetEditor : Editor
{
    private PlanetGenerator _planet;
    private Editor _shapeEditor;

    private void OnEnable()
    {
        _planet = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        using var check = new EditorGUI.ChangeCheckScope();
        base.OnInspectorGUI();
        if (check.changed) 
            _planet.GeneratePlanet();
        GUILayout.Space(2);
        EditorGUILayout.LabelField("1", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Generate Planet"))
        {
            _planet.GeneratePlanet();
            _planet.OnPlanetSettingsUpdated();
            _planet.OnColorSettingsUpdated();
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Remove Planet"))
            _planet.RemovePlanet();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(2);
        
        DrawSettingsEditor(_planet.ObjectSettings, _planet.OnPlanetSettingsUpdated, _planet.OnColorSettingsUpdated, ref _planet.ShapeSettingsFoldout, ref _shapeEditor);
    }

    private static void DrawSettingsEditor(Object planetSettings, Action callbackShapeSettings, Action callbackColorSettings, ref bool foldout, ref Editor editor)
    {
        if (planetSettings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, planetSettings);
            using var check = new EditorGUI.ChangeCheckScope();
            if (foldout)
            {
                CreateCachedEditor(planetSettings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    callbackShapeSettings?.Invoke();
                    callbackColorSettings?.Invoke();
                }
            }
        }
    }
}