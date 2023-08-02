using PlanetSettings;
using System;
using Planet;

namespace CustomEditorWindow
{
    /// <summary>
    /// The WindowPresenter is handling the communication between the WindowView and the ObjectSettings.
    /// It represents the Presenter in the MVP pattern.
    /// </summary>
    public class WindowPresenter
    {
        private readonly WindowLayout _layout;
    
        private ObjectSettings _model;

        public event Action OnDrawUI;
        public event Action OnGUIChanged;
        public event Action OnApplyModified;
        public event Action OnAssetNamesAndPathsReady;

        public WindowPresenter(WindowLayout layout, ObjectGenerator objectGenerator)
        {
            // TODO: ObjectGenerator is null when entering game mode
            _model = objectGenerator.ObjectSettings; 
            _layout = layout;
        }

        public void SubscribeEvents()
        {
            _model.OnSettingsChangedReady += SettingsChanged;
            _layout.OnSettingsUpdated += UpdateModelSettings;
            _layout.OnSettingsInstanceChanged += UpdateModelInstance;
            _layout.OnObjectGeneratorSettingsUpdated += UpdateObjectGeneratorSettings;
        }
    
        public void UnsubscribeEvents()
        {
            _model.OnSettingsChangedReady -= SettingsChanged;
            _layout.OnSettingsUpdated -= UpdateModelSettings;
            _layout.OnSettingsInstanceChanged -= UpdateModelInstance;
            _layout.OnObjectGeneratorSettingsUpdated -= UpdateObjectGeneratorSettings;
        }
    
        // Model dependency:

        private void UpdateModelInstance(ObjectSettings objectSettings)
        {
            _model = objectSettings;
        }
    
        private void UpdateModelSettings(ObjectSettings objectSettings)
        {
            _model.UpdateSettings(objectSettings);
        }

        private void UpdateObjectGeneratorSettings(ObjectGenerator objectGenerator)
        {
            objectGenerator.ObjectSettings = _model;
        }

        private void SettingsChanged()
        {
            _layout.ObjectSettings = _model;
        }
    
        // View dependency:

        public void DrawUI()
        {
            OnDrawUI?.Invoke();
        }
    
        public void GUIChanged()
        {
            OnGUIChanged?.Invoke();
        }
    
        public void ApplyAndModify()
        {
            OnApplyModified?.Invoke();
        }

        public void SetAllAssetsInFolder()
        {
            OnAssetNamesAndPathsReady?.Invoke();
        }
    }
}