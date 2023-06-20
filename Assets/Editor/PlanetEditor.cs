using Object = UnityEngine.Object;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ObjectGenerator))]
public class PlanetEditor : Editor
{
    private ObjectSettings _oldSettings;
    private ObjectGenerator _object;
    private Editor _shapeEditor;
    
    private string _assetName = "";

    private void OnEnable()
    {
        _object = (ObjectGenerator)target;
        _oldSettings = _object.ObjectSettings;
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
        GUILayout.Label("Create New Setting Asset", EditorStyles.boldLabel);
        GUILayout.Space(5);
        _assetName = EditorGUILayout.TextField("Asset Name", _assetName);
        GUILayout.Space(3);
        if(GUILayout.Button("Create Asset"))
            CreateNewSettings(_object);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(2);
    }

    private void CreateNewSettings(ObjectGenerator objectGenerator)
    {
        if (!string.IsNullOrEmpty(_assetName))
        {
            if (AssetDatabase.IsValidFolder("Assets/" + _assetName))
            {
                Debug.LogError("An asset with the same name already exists!");
                return;
            }
            
            var newPlanetSettings = CreateInstance<ObjectSettings>();
            
            if (_oldSettings != null)
            {
                newPlanetSettings.ObjectMaterial = _oldSettings.ObjectMaterial;
                newPlanetSettings.VisualSettings = _oldSettings.VisualSettings;
                newPlanetSettings.FaceRenderMask = _oldSettings.FaceRenderMask;
                newPlanetSettings.NoiseLayers = _oldSettings.NoiseLayers;
            }
            
            const string assetPath = "Assets/ObjectInstances/";
            AssetDatabase.CreateAsset(newPlanetSettings, assetPath + _assetName + ".asset");
            AssetDatabase.SaveAssets();
            objectGenerator.ObjectSettings = newPlanetSettings;
            EditorUtility.SetDirty(objectGenerator);
            
            Debug.Log("Created new asset at: " + assetPath);
            _assetName = "";
            _oldSettings = null;
        }
        else
        {
            Debug.Log("Asset name can´t be empty");
        }
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