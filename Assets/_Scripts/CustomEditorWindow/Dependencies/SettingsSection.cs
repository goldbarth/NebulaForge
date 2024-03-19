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
        private readonly ObjectGeneratorWindow _layout;
    
        private const float ButtonBorderWidth = 15f;
    
        private readonly string[] _tabHeaders;
        private int _tabIndex;
        
        public SettingsSection(ObjectGeneratorWindow layout)
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
            DrawCelestialObjectSelection();
            GUILayout.Space(5);
            DrawTabsAndCurrentTab();
        
            GUILayout.FlexibleSpace();
        
            GUILayout.BeginHorizontal();
            DrawUpdateButton();
            DrawAutoUpdateToggle();
            GUILayout.FlexibleSpace();
            DrawCreateNewCelestialObjectPopOut();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        
            GUILayout.EndArea();
            GUILayout.EndVertical();
        }

        private void DrawSettingsHeader()
        {
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(3);
            GUILayout.Label(_layout.SettingsHeader, LabelStyle.SetCenteredBoldLabel());
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }
        
        private void DrawCelestialObjectSelection()
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
            const string tooltip = "Updates the modified object.";
            if (GUILayout.Button(new GUIContent(TextHolder.UpdateAssetButtonText, tooltip), LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(TextHolder.UpdateAssetButtonText), ButtonBorderWidth)))
                UpdateAssetSettings();
        }
        
        private void DrawAutoUpdateToggle()
        {
            const string tooltip = "Auto-Update is performance intensive and can cause lags when enabled. " +
                                   "It should only be used when necessary. For example, when adjusting settings to see immediate results.";
            _layout.IsAutoUpdate = GUILayout.Toggle(_layout.IsAutoUpdate, new GUIContent(TextHolder.AutoUpdateToggleText, tooltip));
        }
    
        private void UpdateAssetSettings()
        {
            EditorUtility.SetDirty(_layout.ObjectSettings);
            AssetDatabase.SaveAssets();
            _layout.ObjectGenerator.GenerateObject();
        }
        
        private void DrawCreateNewCelestialObjectPopOut()
        {
            const string labelName = "New Celestial Object";
            const string buttonName = TextHolder.CreateButtonName;
            GUILayout.BeginHorizontal();
            GUILayout.Label(labelName);
            if (GUILayout.Button(buttonName, LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(buttonName), ButtonBorderWidth)))
                CreateNewCelestialObjectWindow.ShowWindow();
            GUILayout.EndHorizontal();
        }
    }
}

# endif

