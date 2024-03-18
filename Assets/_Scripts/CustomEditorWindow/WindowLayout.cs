#if UNITY_EDITOR

using CustomEditorWindow.Dependencies;
using PlanetSettings.NoiseSettings;
using HelpersAndExtensions;
using PlanetSettings;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using Planet;
using SolarSystem;

namespace CustomEditorWindow
{
    /// <summary>
    /// The WindowLayout is responsible for the layout and GUI of the EditorWindow.
    /// It represents the View in the MVP pattern.
    /// </summary>
    public class WindowLayout : View
    {
        private CelestialObjectManager _celestialObjectManager;
        public ObjectGenerator ObjectGenerator{ get; private set; }
        public ObjectSettings ObjectSettings { get; set; }
        public string SettingsHeader { get; private set; }

        private WindowPresenter _presenter;
        
        private SidebarSection _sidebarSection;
        private SettingsSection _settingsSection;

        private SerializedObject _serializedObject;
    
        public SerializedProperty MaterialProperty;
        public SerializedProperty ResolutionProperty;
        public SerializedProperty RadiusProperty;
        public SerializedProperty GradientProperty;
        public SerializedProperty ObjectTypeProperty;
        public SerializedProperty NoiseLayerProperty;
        public SerializedProperty EnabledProperty;
        public SerializedProperty UseFirstLayerAsMaskProperty;
        public SerializedProperty NoiseSettingsProperty;
    
        public (string name, string path)[] AssetsInFolder;
        public float SidebarFrameWidth;
        public bool IsAutoUpdate;
        
        private readonly float _updateInterval = 2f;
        private float _lastUpdateTime;
        
        public event Action<ObjectSettings> OnSettingsUpdated;
        public event Action<ObjectSettings> OnSettingsInstanceChanged;
        public event Action<ObjectGenerator> OnObjectGeneratorSettingsUpdated;

        private void OnEnable()
        {
            _celestialObjectManager = CelestialObjectManager.Instance;
            FindAndSetObjectSettings();
        
            _sidebarSection = new SidebarSection(this);
            _settingsSection = new SettingsSection(this);
            _presenter = new WindowPresenter(this, ObjectGenerator);
            
            _presenter.OnDrawUI += DrawLayout;
            _presenter.OnGUIChanged += UpdateSettings;
            _presenter.OnApplyModified += ApplyModifiedProperties;
            _presenter.OnAssetNamesAndPathsReady += SetAssetNamesAndPaths;
        
            ObjectSelectionEventManager.OnObjectSelected += InitializeProperties;
            ObjectSelectionEventManager.OnNoObjectSelected += SetObjectSettingNull;
        
            SetObjectSettings();
            InitializeProperties();

            
            _lastUpdateTime = Time.realtimeSinceStartup;
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.hierarchyChanged += HierarchyChanged;
        }

        private void OnDisable()
        {
            _presenter.UnsubscribeEvents();
            _presenter.OnDrawUI -= DrawLayout;
            _presenter.OnGUIChanged -= UpdateSettings;
            _presenter.OnApplyModified -= ApplyModifiedProperties;
            _presenter.OnAssetNamesAndPathsReady -= SetAssetNamesAndPaths;
        
            ObjectSelectionEventManager.OnObjectSelected -= InitializeProperties;
            ObjectSelectionEventManager.OnNoObjectSelected -= SetObjectSettingNull;
            
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.hierarchyChanged -= HierarchyChanged;
        }

        [MenuItem("Tools/Planet Generator")]
        public static void ShowWindow()
        {
            GetWindow<WindowLayout>(TextHolder.WindowTitle);
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                PlaymodeMessageBox();
                return;
            }
            if (ObjectSettings == null)
            {
                SelectObjectMessageBox();
                return;
            }
            
            GUIChanged();
            DrawUI();
            ApplyAndModify();
        }

        private void OnSelectionChange()
        {
            FindAndSetObjectSettings();
            SetSerializedProperties();
        }
        
        private void HierarchyChanged()
        {
            // if a new game object with with a CelestialObject component is added to the scene
            // add it to the CelestialObjectManager list
            // if a game object with a CelestialObject component is deleted from the scene
            // remove it from the CelestialObjectManager list
            
            // using a hash set to check if the objects are still in the scene. prevents duplicates and is faster
            var sceneObjects = new HashSet<CelestialObject>(FindObjectsOfType<CelestialObject>());
            var objectsToRemove = new List<CelestialObject>();

            // Check if the objects in the scene are still in the list
            foreach (var obj in _celestialObjectManager.CelestialObjects)
            {
                if (!sceneObjects.Contains(obj))
                {
                    objectsToRemove.Add(obj);
                }
                else
                {
                    // Remove the object from the sceneObjects list
                    sceneObjects.Remove(obj);
                }
            }

            // Remove objects that are not in the scene anymore
            foreach (var obj in objectsToRemove)
            {
                _celestialObjectManager.RemoveCelestialObject(obj);
            }

            // Add new objects to the list
            foreach (var obj in sceneObjects)
            {
                _celestialObjectManager.AddCelestialObject(obj);
            }
        }

        #region Editor Update Loop

        private void OnEditorUpdate()
        {
            // Check if the specified update interval has passed
            if (IsTimeToUpdate())
            {
                if (IsAutoUpdate) 
                    ObjectGenerator.UpdateObjectSettings();

                // Mark the GUI as changed to trigger a repaint only if needed
                GUI.changed = true;
                
                _lastUpdateTime = Time.realtimeSinceStartup;
            }
        }

        private bool IsTimeToUpdate()
        {
            return Time.realtimeSinceStartup - _lastUpdateTime >= _updateInterval;
        }

        #endregion

        #region Presenter Dependencies

        private void DrawUI()
        {
            _presenter.DrawUI();
        }
    
        private void GUIChanged()
        {
            if (GUI.changed)
            {
                _presenter.GUIChanged();
            }
        }

        private void ApplyAndModify()
        {
            _presenter.ApplyAndModify();
        }

        public void SetAllAssetsInFolder()
        {
            _presenter.SetAllAssetsInFolder();
        }
        
        private void SetObjectSettings()
        {
            OnSettingsInstanceChanged?.Invoke(ObjectSettings);
            OnSettingsUpdated?.Invoke(ObjectSettings);
        }
        
        private void SetObjectGeneratorSettings()
        {
            OnObjectGeneratorSettingsUpdated?.Invoke(ObjectGenerator);
        }

        #endregion
    
        #region Wrappers

        private void DrawLayout()
        {
            GUILayout.BeginHorizontal();
            DrawSideBarSection();
            DrawSettingsSection();
            GUILayout.EndHorizontal();
        }
        
        private void DrawSideBarSection()
        {
            SetAllAssetsInFolder();
            _sidebarSection.DrawSideBarSection();
        }
        
        private void DrawSettingsSection()
        {
            _settingsSection.DrawSettingsSection();
        }
    
        private void InitializeProperties()
        {
            if (ObjectSettings == null) return;
            SetSettingsSectionWidth();
            SetSettingsSectionHeader();
            SetAllAssetsInFolder();
            SetSerializedProperties();
        }
        
        private void UpdateSettings()
        {
            UpdateSerializedObject();
            SetObjectGeneratorSettings();
            Repaint();
        }
        
        private void UpdateSerializedObject()
        {
            _serializedObject.Update();
        }

        public void UpdateNoiseLayerArray(NoiseLayer[] newLayers)
        {
            ObjectSettings.NoiseLayers = newLayers;
            _serializedObject = new SerializedObject(ObjectSettings);
        }
        
        private void ApplyModifiedProperties()
        {
            _serializedObject.ApplyModifiedProperties();
        }
        
        private void FindAndSetObjectSettings()
        {
            ObjectGenerator = FindSelectedObjectGetComponent();
            if(ObjectGenerator == null) return;
            ObjectSettings = ObjectGenerator.ObjectSettings;
        }
        
        private void SetAssetNamesAndPaths()
        {
            AssetsInFolder = GetAssetNamesAndPaths();
        }

        private void SetObjectSettingNull()
        {
            ObjectSettings = null;
        }

        public ObjectSettings SetSelectedAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<ObjectSettings>(path);
        }
    
        public float SetSettingsSectionWidth()
        {
            return position.width - SidebarFrameWidth - 10;
        }
    
        public void SetNoiseLayerProperty(int layerIndex)
        {
            NoiseLayerProperty = 
                _serializedObject.FindProperty("NoiseLayers").GetArrayElementAtIndex(layerIndex);
        }

        private void SetSettingsSectionHeader()
        {
            if (ObjectSettings == null) return;
            SettingsHeader = $"Settings [{ObjectSettings.name}]";
        }
        
        private void SetSerializedProperties()
        {
            _serializedObject = new SerializedObject(ObjectSettings);
            ObjectTypeProperty = _serializedObject.FindProperty("ObjectType");
            MaterialProperty = _serializedObject.FindProperty("Material");
            ResolutionProperty = _serializedObject.FindProperty("Resolution");
            RadiusProperty = _serializedObject.FindProperty("Radius");
            GradientProperty = _serializedObject.FindProperty("Gradient");
            NoiseLayerProperty = _serializedObject.FindProperty("NoiseLayers");
        }
        
        public void AttachDataToAsset(ObjectSettings selectedAsset)
        {
            ObjectGenerator.ObjectSettings = selectedAsset;
            ObjectSettings = selectedAsset;
            SetObjectSettings();
            SetSettingsSectionHeader();
            SetSerializedProperties();
            SetAllAssetsInFolder();

            EditorUtility.SetDirty(ObjectSettings);
            ObjectGenerator.GenerateObject();
        }
    
        private void SelectObjectMessageBox()
        {
            EditorGUILayout.HelpBox("─=≡Σ((( つ◕ل͜◕)つ No GameObject was selected in the Hierarchy. " +
                                    "Please select a GameObject to access the settings.", MessageType.Info);
        }
        
        private void PlaymodeMessageBox()
        {
            EditorGUILayout.HelpBox("The game is in play mode. " +
                                    "In play mode the editor is disabled. You can only change the settings in editor mode." +
                                    " ─=≡Σ((( つ◕ل͜◕)つ", MessageType.Info);
        }
        
        #endregion

        private ObjectGenerator FindSelectedObjectGetComponent()
        {
            var selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject != null)
            {
                var script = selectedGameObject.GetComponent<ObjectGenerator>();
                if (script != null) return script;

                var scriptsInChildren = selectedGameObject.GetComponentsInChildren<ObjectGenerator>();
                if (scriptsInChildren.Length > 0) return scriptsInChildren[0];
            }
            else
            {
                ObjectGenerator = null;
                Debug.LogWarningFormat("No GameObject selected in the Hierarchy. " +
                                       "Please select a GameObject first. This happened at: {0:f}. Isn´t it cool?", DateTime.Now);
            }
        
            return null;
        }
    
        /// <summary>
        /// Tuple with asset name and asset path. Less than three.
        /// </summary>
        /// <returns>Returns a tuple array with all asset. Each tuple contains an asset name and path.</returns>
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
}

#endif