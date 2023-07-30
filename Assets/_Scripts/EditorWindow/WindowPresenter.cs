using System;

/// <summary>
/// The WindowPresenter is responsible for the logic of the EditorWindow.
/// It represents the Presenter in the MVP pattern.
/// </summary>
public class WindowPresenter
{
    private readonly WindowView _view;
    
    private ObjectSettings _model;

    public event Action OnDrawUI;
    public event Action OnGUIChanged;
    public event Action OnApplyModified;
    public event Action OnAssetNamesAndPathsReady;

    public WindowPresenter(WindowView view, ObjectGenerator objectGenerator)
    {
        _model = objectGenerator.ObjectSettings;
        _view = view;
    }

    public void SubscribeEvents()
    {
        _model.OnSettingsChangedReady += SettingsChanged;
        _view.OnSettingsUpdated += UpdateModelSettings;
        _view.OnSettingsInstanceChanged += UpdateModelInstance;
        _view.OnObjectGeneratorSettingsUpdated += UpdateObjectGeneratorSettings;
    }
    
    public void UnsubscribeEvents()
    {
        _model.OnSettingsChangedReady -= SettingsChanged;
        _view.OnSettingsUpdated -= UpdateModelSettings;
        _view.OnSettingsInstanceChanged -= UpdateModelInstance;
        _view.OnObjectGeneratorSettingsUpdated -= UpdateObjectGeneratorSettings;
    }
    
    // Model dependency

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
    
    // View dependency

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