using UnityEditor;
using UnityEngine;

public class GeneralTab
{
    private readonly WindowView _view;

    public GeneralTab(WindowView view)
    {
        _view = view;
    }

    public void DrawGeneralSettingsTab()
    {
        DrawGeneralSettingsHeader();
        DrawObjectSettingsField();
        EditorGUILayout.PropertyField(_view.MaterialProperty, true);
        EditorGUILayout.PropertyField(_view.ResolutionProperty);
        EditorGUILayout.PropertyField(_view.RadiusProperty);
        if (_view.ObjectSettings.ObjectType == ObjectType.TerrestrialBody)
            EditorGUILayout.PropertyField(_view.GradientProperty);
    }

    private void DrawObjectSettingsField()
    {
        _view.ObjectSettings =
            (ObjectSettings)EditorGUILayout.ObjectField(TextHolder.CurrentAssetLabel, _view.ObjectSettings, typeof(ObjectSettings), false);
    }

    private void DrawGeneralSettingsHeader()
    {
        GUILayout.Label(TextHolder.GeneralSettingsHeader, LabelStyle.SetCenteredBoldLabel());
    }
}