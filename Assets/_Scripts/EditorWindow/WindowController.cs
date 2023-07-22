using UnityEditor;
using UnityEngine;

/// <summary>
/// The WindowController is responsible for the logic of the EditorWindow.
/// It represents the Controller in the MVC pattern.
/// </summary>
public class WindowController
{
    private readonly WindowView _view;
    

    public WindowController(WindowView view)
    {
        _view = view;
    }
    
    public void DrawProperties()
    {
        _view.DrawProperties();
    }
    
    public void DrawNoiseLayer()
    {
        _view.DrawNoiseLayer();
    }
    
    public void DrawButtons()
    {
        _view.DrawButtons();
    }
    
    public void AddNoiseLayer()
    {
        _view.AddNoiseLayer();
    }

    public void RemoveLastNoiseLayer()
    {
        _view.RemoveLastNoiseLayer();
    }

    public void OnGUIChanged()
    {
        _view.OnGUIChanged();
    }
}