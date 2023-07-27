using System;

/// <summary>
/// The WindowController is responsible for the logic of the EditorWindow.
/// It represents the Controller in the MVC pattern.
/// Using events, to pass information from the View to the Controller.
/// It can be relevant in the future.
/// </summary>
public class WindowController
{
    public event Action OnDrawUI;
    public event Action OnGUIChanged;
    public event Action OnApplyModified;
    public event Action OnAssetNamesAndPathsReady;

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