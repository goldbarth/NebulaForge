#if UNITY_EDITOR

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
        private readonly ObjectGenerator _objectGenerator;
        private readonly ObjectGeneratorWindow _view;
        
        private ObjectSettings _model;

        public event Action OnDrawUI;
        public event Action OnGUIChanged;
        public event Action OnApplyModified;
        public event Action OnAssetNamesAndPathsReady;

        public WindowPresenter(ObjectGeneratorWindow view, ObjectGenerator objectGenerator)
        {
            if (objectGenerator == null) return;
            
            _objectGenerator = objectGenerator;
            _model = _objectGenerator.ObjectSettings; 
            _view = view;
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _model.OnSettingsChangedReady += SettingsChanged;
            _view.OnSettingsUpdated += UpdateModelSettings;
            _view.OnSettingsInstanceChanged += UpdateModelInstance;
            _view.OnObjectGeneratorSettingsUpdated += UpdateObjectGeneratorSettings;
        }
    
        public void UnsubscribeEvents()
        {
            if (_objectGenerator == null) return;
            
            _model.OnSettingsChangedReady -= SettingsChanged;
            _view.OnSettingsUpdated -= UpdateModelSettings;
            _view.OnSettingsInstanceChanged -= UpdateModelInstance;
            _view.OnObjectGeneratorSettingsUpdated -= UpdateObjectGeneratorSettings;
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
            _view.ObjectSettings = _model;
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

#endif