namespace HelpersAndExtensions
{
    /// <summary>
    /// A static class that holds all the paths where all the ObjectSettings are stored.
    /// </summary>
    public static class FolderPath
    {
        public const string RootInstances = "Assets/ObjectInstances/";
        public const string Origin = "Assets/_Scripts/ObjectSettings/";
    
        public static string NewAssetFolder(string name) => $"{RootInstances}{name}";
        public static string GetAssetFolder(string name) => $"{RootInstances}{name}";
        public static string OriginAssetPath => $"{Origin}ObjectSettings.asset";
    }
}