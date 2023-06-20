using Object = UnityEngine.Object;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ObjectGenerator))]
public class PlanetEditor : Editor
{
    private ObjectGenerator _object;
    private Editor _shapeEditor;

    private void OnEnable()
    {
        _object = (ObjectGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        using var check = new EditorGUI.ChangeCheckScope();
        base.OnInspectorGUI();
        if (check.changed) 
            _object.GeneratePlanet();
        
        ButtonLayout();

        DrawSettingsEditor(_object.ObjectSettings, _object.OnPlanetSettingsUpdated, _object.OnColorSettingsUpdated, ref _object.ShapeSettingsFoldout, ref _shapeEditor);
    }

    private void ButtonLayout()
    {
        GUILayout.Space(2);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Generate Planet"))
        {
            _object.GeneratePlanet();
            _object.OnPlanetSettingsUpdated();
            _object.OnColorSettingsUpdated();
        }
        
        GUILayout.Space(2);
        if (GUILayout.Button("Remove Planet"))
            _object.RemovePlanet();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        GUILayout.Space(2);
        if(GUILayout.Button("Create New Settings Asset"))
            CreateNewSettings(_object);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(2);
    }

    private void CreateNewSettings(ObjectGenerator @object)
    {
        var newPlanetSettings = CreateInstance<ObjectSettings>();
        var assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ObjectInstances/NewObjectSettings.asset");
        AssetDatabase.CreateAsset(newPlanetSettings, assetPath);
        AssetDatabase.SaveAssets();
        
        @object.ObjectSettings = newPlanetSettings;
        EditorUtility.SetDirty(@object);
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