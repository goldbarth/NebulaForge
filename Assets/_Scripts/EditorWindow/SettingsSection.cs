using UnityEditor;
using UnityEngine;

public class SettingsSection
{
    private readonly GeneralTab _generalTab;
    private readonly SurfaceTab _surfaceTab;
    private readonly WindowView _view;
    
    private readonly string[] _tabHeaders;
    private int _tabIndex;

    public SettingsSection(WindowView view)
    {
        _view = view;
        _generalTab = new GeneralTab(_view);
        _surfaceTab = new SurfaceTab(_view);
        _tabHeaders = new[] { TextHolder.GeneralSettingsHeader, TextHolder.SurfaceSettingsHeader };
    }

    public void DrawSettingsSection()
    {
        const float separationLine = 5f;
        var areaRect = new Rect(_view.SidebarFrameWidth + separationLine, 0, _view.SetSettingsSectionWidth(), _view.position.height);
        
        GUI.Box(areaRect, string.Empty);

        GUILayout.BeginVertical();
        GUILayout.BeginArea(areaRect);
        
        DrawSettingsHeader();
        DrawTabsAndCurrentTab();
        
        GUILayout.FlexibleSpace();
        
        GUILayout.BeginHorizontal();
        DrawSaveButton();
        GUILayout.FlexibleSpace();
        DrawDeleteButton();
        GUILayout.EndHorizontal();
        
        GUILayout.EndArea();
        GUILayout.EndVertical();
    }

    private void DrawSettingsHeader()
    {
        EditorGUILayout.Space(3);
        GUILayout.Label(_view.SettingsHeader, LabelStyle.SetCenteredBoldLabel());
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
    }

    private void DrawTabsAndCurrentTab()
    {
        _tabIndex = GUILayout.Toolbar(_tabIndex, _tabHeaders);
        EditorGUILayout.Space(10);
        
        switch (_tabIndex)
        {
            case 0:
                _generalTab.DrawGeneralSettingsTab();
                break;
            case 1:
                _surfaceTab.DrawElevationSettingsTab();
                break;
        }
    }
    
    private void UpdateAssetSettings()
    {
        EditorUtility.SetDirty(_view.ObjectSettings);
        AssetDatabase.SaveAssets();
        _view.ObjectGenerator.GeneratePlanet();
    }
    
    private void DrawSaveButton()
    {
        if (GUILayout.Button(TextHolder.UpdateAssetButtonText))
            UpdateAssetSettings();
    }

    private void DrawDeleteButton()
    {
        if (GUILayout.Button(TextHolder.DeleteAssetButtonText))
            DeleteSelectedAsset();
    }

    private void DeleteSelectedAsset()
    {
        var currentAsset = _view.ObjectSettings;
        var assetPath = currentAsset != null ? AssetDatabase.GetAssetPath(currentAsset) : string.Empty;
        var folderPath = assetPath.Substring(0, assetPath.LastIndexOf('/'));
        
        if (EditorUtility.DisplayDialog("Delete Asset", $"Are you sure you want to delete {currentAsset}?", "Yes", "No"))
        {
            // delete asset folder
            AssetDatabase.DeleteAsset(folderPath);
            Debug.Log($"Deleted asset with folder at: {folderPath}");
            AssetDatabase.Refresh();
            
            // load first asset in folder
            var firstAsset = AssetDatabase.FindAssets("t:ObjectSettings", new[] { FolderPath.RootInstances });
            var selectedAsset = AssetDatabase.LoadAssetAtPath<ObjectSettings>(AssetDatabase.GUIDToAssetPath(firstAsset[0]));
            Debug.Log($"Asset loaded at folder: {selectedAsset}");
            _view.AttachDataToAsset(selectedAsset);
            _view.ObjectGenerator.GeneratePlanet();
        }
    }
}