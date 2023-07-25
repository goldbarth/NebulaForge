using UnityEditor;
using UnityEngine;

public class SidebarSection
{
    private readonly WindowView _view;

    private readonly ObjectType[] _objectTypes;
    private Vector2 _leftScrollPosition;

    public SidebarSection(WindowView view)
    {
        _view = view;
        _objectTypes = new[] { ObjectType.TerrestrialBody, ObjectType.SolidSphere };
    }

    public void DrawSideBarSection()
    {
        var areaRect = new Rect(0, 0, _view.SidebarFrameWidth, _view.position.height);

        GUILayout.BeginVertical();
        GUILayout.BeginArea(areaRect);
        
        GUI.Box(areaRect, string.Empty);
        
        BeginDrawLeftScrollView();
        DrawSidebarHeader();
        DrawAssetButtonsInOrder(_view.AssetsInFolder);

        GUILayout.EndArea();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawAssetButtonsInOrder((string name, string path)[] assetsInFolder)
    {
        DrawSidebarSubHeader1();
        DrawAssetButtonsByType(assetsInFolder, _objectTypes[1]);
        DrawSidebarSubHeader2();
        DrawAssetButtonsByType(assetsInFolder, _objectTypes[0]);

    }

    private void DrawAssetButtonsByType((string name, string path)[] assetsInFolder, ObjectType objectTypes)
    {
        const float buttonBorderWith = 10f;
        var maxButtonWidth = MaxButtonWidth(assetsInFolder);
        var defaultStyle = SetButtonDefaultStyle(maxButtonWidth, buttonBorderWith);
        
        foreach (var asset in assetsInFolder)
        {
            var selectedAsset = SetSelectedAsset(asset);
            if (selectedAsset.ObjectType == objectTypes) continue;

            DrawAssetButton(selectedAsset, asset, defaultStyle);
        }
    }

    private void DrawAssetButton(ObjectSettings selectedAsset, (string name, string path) asset, GUIStyle buttonStyle)
    {
        if (GUILayout.Button(asset.name, buttonStyle))
        {
            if (selectedAsset == null)
            {
                Debug.LogWarning($"Something went wrong loading the asset from the path {asset.path}.");
                return;
            }

            _view.ObjectSettings = selectedAsset;
        }

        EditorGUILayout.Space(1);
    }
    
    private GUIStyle SetButtonDefaultStyle(float maxButtonWidth, float buttonBorderWith)
    {
        var defaultStyle = new GUIStyle(GUI.skin.button);
        defaultStyle.normal.textColor = Color.white;
        defaultStyle.fixedWidth = maxButtonWidth + buttonBorderWith;

        // TODO: Find out why the background color is not applied.
        defaultStyle.normal.background = CreateColoredTexture(2, 2, Color.gray);
        defaultStyle.active.background = CreateColoredTexture(2, 2, Color.gray);
        return defaultStyle;
    }

    private float MaxButtonWidth((string name, string path)[] assetsInFolder)
    {
        const float frameBorderWidth = 15f;
        var maxButtonWidth = 0f;
        
        foreach (var asset in assetsInFolder)
        {
            var assetNameWidth = GUI.skin.button.CalcSize(new GUIContent(asset.name)).x;
            maxButtonWidth = Mathf.Max(maxButtonWidth, assetNameWidth);
            _view.SidebarFrameWidth = maxButtonWidth + frameBorderWidth;
        }

        return maxButtonWidth;
    }

    private static Texture2D CreateColoredTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];
        for (int colorsIndex = 0; colorsIndex < pixels.Length; colorsIndex++)
            pixels[colorsIndex] = color;
        
        var texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private static void DrawSidebarHeader()
    {
        EditorGUILayout.Space(3);
        GUILayout.Label(TextHolder.SidebarHeader, LabelStyle.SetCenteredBoldLabel());
        EditorGUILayout.Space(3);
    }

    private void BeginDrawLeftScrollView()
    {
        _leftScrollPosition = GUILayout.BeginScrollView(_leftScrollPosition, GUILayout.Width(_view.SidebarFrameWidth + 2));
    }
    
    private static ObjectSettings SetSelectedAsset((string name, string path) asset)
    {
        return AssetDatabase.LoadAssetAtPath<ObjectSettings>(asset.path);
    }

    private static void DrawSidebarSubHeader2()
    {
        GUILayout.Label(TextHolder.SidebarSubHeader2, LabelStyle.SetCenteredMiniLabel());
    }

    private static void DrawSidebarSubHeader1()
    {
        GUILayout.Label(TextHolder.SidebarSubHeader1, LabelStyle.SetCenteredMiniLabel());
    }
}