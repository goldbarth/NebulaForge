public static class FolderPath
{
    public const string Root = "Assets/ObjectInstances/";
    
    public static string NewAssetFolder(string name)
    {
        return $"{Root}{name}";
    }
}