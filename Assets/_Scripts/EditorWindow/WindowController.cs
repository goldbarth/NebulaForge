using System;

/// <summary>
/// The WindowController is responsible for the logic of the EditorWindow.
/// It represents the Controller in the MVC pattern.
/// Using events, to pass information from the View to the Controller.
/// It can be relevant in the future.
/// </summary>
public class WindowController
{
    private readonly WindowView _view;
    
    public event EventHandler GUIChanged;
    public event EventHandler UpdateSerializedObject;
    public event EventHandler DrawSideBarSection;
    public event EventHandler DrawSettingsSection;
    
    public WindowController(WindowView view)
    {
        _view = view;
    }
    
    public void OnUpdate()
    {
        UpdateSerializedObject?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnGUIChanged()
    {
        GUIChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnDrawSideBarSection()
    {
        DrawSideBarSection?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnDrawSettingsSection()
    {
        DrawSettingsSection?.Invoke(this, EventArgs.Empty);
    }
    
    // public Action OnGUIChanged => _view.OnGUIChanged;
    // public Action OnUpdate => _view.UpdateSerializedObject;
    // public Action DrawSideBarSection => _view.DrawSideBarSection;
    // public Action DrawSettingsSection => _view.DrawSettingsSection;
    public ObjectSettings GetObjectSettings()
    {
        throw new NotImplementedException();
    }
}