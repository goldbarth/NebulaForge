# if UNITY_EDITOR

using PlanetSettings;
using UnityEditor;
using HelpersAndExtensions;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class SettingsSection
    {
        private readonly SelectionArea _selectionArea;
        private readonly GeneralTab _generalTab;
        private readonly SurfaceTab _surfaceTab;
        private readonly WindowLayout _layout;
    
        private const float ButtonBorderWidth = 15f;
    
        private readonly string[] _tabHeaders;
        private int _tabIndex;

        public SettingsSection(WindowLayout layout)
        {
            _layout = layout;
            _selectionArea = new SelectionArea();
            _generalTab = new GeneralTab(_layout);
            _surfaceTab = new SurfaceTab(_layout);
            _tabHeaders = new[] { TextHolder.GeneralSettingsHeader, TextHolder.SurfaceSettingsHeader };
        }

        public void DrawSettingsSection()
        {
            const float separationLine = 5f;
            var areaRect = new Rect(_layout.SidebarFrameWidth + separationLine, 0, _layout.SetSettingsSectionWidth(), _layout.position.height);
        
            GUI.Box(areaRect, string.Empty);

            GUILayout.BeginVertical();
            GUILayout.BeginArea(areaRect);
        
            DrawSettingsHeader();
            DrawSelectionArea();
            GUILayout.Space(5);
            DrawTabsAndCurrentTab();
        
            GUILayout.FlexibleSpace();
        
            GUILayout.BeginHorizontal();
            DrawUpdateButton();
            DrawAutoUpdateToggle();
            GUILayout.FlexibleSpace();
            DrawDeleteButton();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        
            GUILayout.EndArea();
            GUILayout.EndVertical();
        }

        private void DrawSettingsHeader()
        {
            EditorGUILayout.Space(3);
            GUILayout.Label(_layout.SettingsHeader, LabelStyle.SetCenteredBoldLabel());
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }
        
        private void DrawSelectionArea()
        {
            _selectionArea.DrawSelectionArea();
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

        private void DrawUpdateButton()
        {
            if (GUILayout.Button(TextHolder.UpdateAssetButtonText, 
                    LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(TextHolder.UpdateAssetButtonText), ButtonBorderWidth)))
                UpdateAssetSettings();
        }
        
        private void DrawAutoUpdateToggle()
        {
            var tooltip = "Auto-Update is performance intensive and can cause lags when enabled. " +
                          "It should only be used when necessary. For example, when adjusting settings to see immediate results.";
            _layout.IsAutoUpdate = GUILayout.Toggle(_layout.IsAutoUpdate, new GUIContent(TextHolder.AutoUpdateToggleText, tooltip));
        }

        private void DrawDeleteButton()
        {
            if (GUILayout.Button(TextHolder.DeleteAssetButtonText,
                    LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(TextHolder.DeleteAssetButtonText), ButtonBorderWidth)))
                DeleteSelectedAsset();
        }
    
        private void UpdateAssetSettings()
        {
            EditorUtility.SetDirty(_layout.ObjectSettings);
            AssetDatabase.SaveAssets();
            _layout.ObjectGenerator.GenerateObject();
        }

        private void DeleteSelectedAsset()
        {
            var currentAsset = _layout.ObjectSettings;
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
                _layout.SetAllAssetsInFolder();
                _layout.AttachDataToAsset(selectedAsset);
                _layout.ObjectGenerator.GenerateObject();
            }
        }
    }
}

# endif

