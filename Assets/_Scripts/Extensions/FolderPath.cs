public static class FolderPath
{
    public const string RootInstances = "Assets/ObjectInstances/";
    public const string Origin = "Assets/_Scripts/ObjectSettings/";
    
    public static string NewAssetFolder(string name) => $"{RootInstances}{name}";
    public static string OriginAssetPath => $"{Origin}ObjectSettings.asset";
}