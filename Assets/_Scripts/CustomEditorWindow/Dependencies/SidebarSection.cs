# if UNITY_EDITOR

using PlanetSettings;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class SidebarSection
    {
        private readonly WindowLayout _layout;

        private readonly ObjectType[] _objectTypes;
        private Vector2 _leftScrollPosition;

        private const float ButtonBorderWidth = 10f;

        public SidebarSection(WindowLayout layout)
        {
            _layout = layout;
            _objectTypes = new[] { ObjectType.TerrestrialBody, ObjectType.SolidSphere };
        }

        public void DrawSideBarSection()
        {
            const float separationLine = 2.5f;
            var areaRect = new Rect(separationLine, 0, _layout.SidebarFrameWidth, _layout.position.height);

            GUILayout.BeginVertical();
            GUILayout.BeginArea(areaRect);
        
            GUI.Box(areaRect, string.Empty);
        
            BeginDrawLeftScrollView();
            DrawSidebarHeader();
            DrawAssetButtonsInOrder(_layout.AssetsInFolder);
        
            GUILayout.FlexibleSpace();
        
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawCreateNewAssetPopOut();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.EndArea();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawCreateNewAssetPopOut()
        {
            var buttonName = "New Asset";
            if (GUILayout.Button(buttonName, LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(buttonName), ButtonBorderWidth)))
                CreateNewAssetWindow.ShowWindow();
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
            var maxButtonWidth = MaxButtonWidth(assetsInFolder);
            var defaultStyle = LabelStyle.SetDefaultButtonStyle(maxButtonWidth, ButtonBorderWidth);
        
            foreach (var asset in assetsInFolder)
            {
                var selectedAsset = _layout.SetSelectedAsset(asset.path);
                if (selectedAsset.ObjectType == objectTypes) continue;

                // Set the background color of the selected asset button.
                defaultStyle.normal.background = LabelStyle.CreateColoredTexture(2, 2, 
                    selectedAsset == _layout.ObjectSettings ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.gray);

                DrawAssetButton(selectedAsset, asset, defaultStyle);
            }
        }

        private void DrawAssetButton(ObjectSettings selectedAsset, (string name, string path) asset, GUIStyle buttonStyle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(6f);
        
            if (GUILayout.Button(asset.name, buttonStyle))
            {
                if (selectedAsset == null)
                {
                    Debug.LogWarning($"Asset Button couldn´t be drawn. Something went wrong loading the asset from the path {asset.path}.");
                    return;
                }

                _layout.AttachDataToAsset(selectedAsset);
            }
        
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(1);
        }

        private float MaxButtonWidth((string name, string path)[] assetsInFolder)
        {
            const float frameBorderWidth = 20f;
            var maxButtonWidth = 0f;
        
            foreach (var asset in assetsInFolder)
            {
                var assetNameWidth = GUI.skin.button.CalcSize(new GUIContent(asset.name)).x;
                maxButtonWidth = Mathf.Max(maxButtonWidth, assetNameWidth);
                _layout.SidebarFrameWidth = maxButtonWidth + frameBorderWidth;
            }

            return maxButtonWidth;
        }

        private static void DrawSidebarHeader()
        {
            EditorGUILayout.Space(3);
            GUILayout.Label(TextHolder.SidebarHeader, LabelStyle.SetCenteredBoldLabel());
            EditorGUILayout.Space(3);
        }

        private void BeginDrawLeftScrollView()
        {
            _leftScrollPosition = GUILayout.BeginScrollView(_leftScrollPosition, GUILayout.Width(_layout.SidebarFrameWidth + 3));
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
}

#endif

