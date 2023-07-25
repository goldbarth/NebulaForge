using System;
using UnityEditor;

/// <summary>
/// The WindowController is responsible for the logic of the EditorWindow.
/// It represents the Controller in the MVC pattern.
/// Using events, to pass information from the View to the Controller.
/// It can be relevant in the future.
/// </summary>
public class WindowController
{
    public ObjectSettings ObjectSettings { get; set; }
    public WindowController()
    { }

    // Events for better communication between the View and Controller and decoupling.
    public delegate void GUIChangedEventHandler(object sender, EventArgs e);
    public delegate void UpdateSerializedObjectEventHandler(object sender, EventArgs e);
    public delegate void DrawSideBarSectionEventHandler((string name, string path)[] assetNamesAndPaths, EventArgs e);
    public delegate void DrawSettingsSectionEventHandler(object sender, EventArgs e);
    public delegate void AssetNamesAndPathsReadyEventHandler((string name, string path)[] assetNamesAndPaths, EventArgs e);
    
    public GUIChangedEventHandler GUIChanged;
    public UpdateSerializedObjectEventHandler UpdateSerializedObject;
    public DrawSideBarSectionEventHandler DrawSideBarSection;
    public DrawSettingsSectionEventHandler DrawSettingsSection;
    public AssetNamesAndPathsReadyEventHandler AssetNamesAndPathsReady;


    public void OnUpdate()
    {
        UpdateSerializedObject?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnGUIChanged()
    {
        GUIChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnDrawSideBarSection((string name, string path)[] assetNamesAndPaths)
    {
        DrawSideBarSection?.Invoke(assetNamesAndPaths, EventArgs.Empty);
    }
    
    public void OnDrawSettingsSection()
    {
        DrawSettingsSection?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnGetAllAssetNamesAndPaths()
    {
        var assetNamesAndPaths = GetAssetNamesAndPaths();
        AssetNamesAndPathsReady?.Invoke(assetNamesAndPaths, EventArgs.Empty);
    }

    /// <summary>
    /// Tuple with asset name and asset path. Less than three.
    /// </summary>
    /// <returns>Returns a tuple array with all asset. Each tuple contains an asset name and path</returns>
    private (string name, string path)[] GetAssetNamesAndPaths()
    {
        // Get all asset paths from the folder.
        const string folderPath = FolderPath.RootInstances;
        var assetPaths = AssetDatabase.FindAssets("t:ObjectSettings", new[] { folderPath });
    
        // Create a tuple array with the asset name and path.
        var assetNamesAndPaths = new (string name, string path)[assetPaths.Length];
        
        // Fill the tuple array with the asset name and path.
        for (var assetIndex = 0; assetIndex < assetPaths.Length; assetIndex++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[assetIndex]);
            var assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            assetNamesAndPaths[assetIndex] = (assetName, assetPath);
        }
        
        return assetNamesAndPaths;
    }
}