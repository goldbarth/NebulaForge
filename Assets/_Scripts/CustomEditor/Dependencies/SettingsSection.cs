using System;
using CustomEditorWindow;
using UnityEditor;
using UnityEngine;

namespace EditorWindowDependencies
{
    public class SettingsSection
    {
        private readonly GeneralTab _generalTab;
        private readonly SurfaceTab _surfaceTab;
        private readonly WindowView _view;
    
        private const float buttonBorderWidth = 15f;
    
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

        private void DrawUpdateButton()
        {
            if (GUILayout.Button(TextHolder.UpdateAssetButtonText, 
                    LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(TextHolder.UpdateAssetButtonText), buttonBorderWidth)))
                UpdateAssetSettings();
        }
        
        private void DrawAutoUpdateToggle()
        {
            var tooltip = "Auto-Update is performance intensive and can cause lags when enabled. " +
                          "It should only be used when necessary. For example, when adjusting settings to see immediate results.";
            _view.ShowInstantChanges = GUILayout.Toggle(_view.ShowInstantChanges, new GUIContent(TextHolder.AutoUpdateToggleText, tooltip));
        }

        private void DrawDeleteButton()
        {
            if (GUILayout.Button(TextHolder.DeleteAssetButtonText,
                    LabelStyle.SetButtonDefaultStyle(LabelStyle.MaxButtonWidth(TextHolder.DeleteAssetButtonText), buttonBorderWidth)))
                DeleteSelectedAsset();
        }
    
        private void UpdateAssetSettings()
        {
            EditorUtility.SetDirty(_view.ObjectSettings);
            AssetDatabase.SaveAssets();
            _view.ObjectGenerator.GenerateObject();
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
                _view.SetAllAssetsInFolder();
                _view.AttachDataToAsset(selectedAsset);
                _view.ObjectGenerator.GenerateObject();
            }
        }
    }
}

