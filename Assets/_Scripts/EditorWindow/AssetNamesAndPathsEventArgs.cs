using System;

public class AssetNamesAndPathsEventArgs : EventArgs
{
    public (string name, string path)[] AssetNamesAndPaths { get; }

    public AssetNamesAndPathsEventArgs((string name, string path)[] assetNamesAndPaths)
    {
        AssetNamesAndPaths = assetNamesAndPaths;
    }
}