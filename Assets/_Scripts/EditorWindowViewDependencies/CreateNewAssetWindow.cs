using UnityEditor;
using UnityEngine;

public class CreateNewAssetWindow : EditorWindow
{
    private readonly CreateNewAsset _createNewAsset = new();
    private WindowView _view;
    
    private ObjectGenerator _objectGenerator;

    private string _assetName = string.Empty;

    private void OnEnable()
    {
        _view = CreateInstance<WindowView>();
        _objectGenerator = _view.ObjectGenerator;
    }

    public static void ShowWindow()
    {
        var window = GetWindow<CreateNewAssetWindow>("Create New Asset");
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 400);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Enter a name for the new asset:");
        EditorGUILayout.Space(5);
        _assetName = EditorGUILayout.TextField("Asset Name", _assetName);
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Create"))
            _createNewAsset.CreateAsset(_objectGenerator, _view, _assetName);
        
        if (_createNewAsset.IsAssetNameEmpty)
        {
            EditorGUILayout.HelpBox("Asset name can´t be empty! You need to enter a name for the asset.", MessageType.Warning);
        }
        
        if (!_createNewAsset.IsAssetNameValid)
        {
            EditorGUILayout.HelpBox("An asset with the same name already exists. Please enter a new name.", MessageType.Warning);
        }
        
        GUILayout.FlexibleSpace();
        
        EditorGUILayout.HelpBox("For now, the new created asset is a copy of the current selected asset. " +
                                "In the future it will be possible to define properties. Stay tuned. (ﾉ◕ヮ◕)ﾉ*:・ﾟ✧", MessageType.Info, true);
        if (GUILayout.Button("Close"))
            Close();
    }
}