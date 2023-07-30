#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class CreateNewAssetWindow : EditorWindow
{
    private readonly CreateNewAsset _createNewAsset = new();
    
    private ObjectGenerator _objectGenerator;
    private WindowView _view;
    

    private string _assetName = string.Empty;
    private const string CreateButtonName = "Create";
    private const string CloseButtonName = "Close";

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
        const float buttonBorderWidth = 15f;
        EditorGUILayout.LabelField("Enter a name for the new asset:");
        EditorGUILayout.Space(10);
        _assetName = EditorGUILayout.TextField("Asset Name", _assetName);
        EditorGUILayout.Space(7);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(CreateButtonName, LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(CreateButtonName), buttonBorderWidth)))
            _createNewAsset.CreateAsset(_objectGenerator, _view, _assetName);
        GUILayout.EndHorizontal();
        
        if (_createNewAsset.IsAssetNameEmpty)
            EditorGUILayout.HelpBox("Asset name can´t be empty! " +
                                    "You need to enter a name for the asset.", MessageType.Warning);
        
        
        if (!_createNewAsset.IsAssetNameValid)
            EditorGUILayout.HelpBox("An asset with the same name already exists. " +
                                    "Please enter a new name.", MessageType.Warning);

        GUILayout.FlexibleSpace();
        
        EditorGUILayout.HelpBox("For now, the new created asset is a copy of the current selected asset. " +
                                "In the future it will be possible to define properties. Stay tuned. (ﾉ◕ヮ◕)ﾉ*:・ﾟ✧", MessageType.Info, true);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(CloseButtonName, LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(CloseButtonName), buttonBorderWidth)))
            Close();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}

#endif