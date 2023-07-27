using UnityEditor;
using UnityEngine;

public class DrawNoiseLayerOptions
{
    private readonly WindowView _view;

    public DrawNoiseLayerOptions( WindowView view)
    {
        _view = view;
    }

    public void DrawNoiseLayerOptionButtons()
    {
        var buttonWidth = _view.SetSettingsSectionWidth() * 0.5f - 10;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(TextHolder.AddLayerButtonText, GUILayout.Width(buttonWidth)))
            AddNoiseLayer();
        
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button(TextHolder.RemoveLayerButtonText, GUILayout.Width(buttonWidth)))
            RemoveLastNoiseLayer();

        GUILayout.EndHorizontal();
    }

    private void AddNoiseLayer()
    {
        if (_view.ObjectSettings.NoiseLayers.Length >= 3)
        {
            Debug.LogWarning("Cannot add more than 3 NoiseLayers.");
            return;
        }

        // Create a new array with the length of the current array + 1.
        var currentLayers = _view.ObjectSettings.NoiseLayers;
        var newLayers = new NoiseLayer[currentLayers.Length + 1];

        // Copy the current array to the new array.
        currentLayers.CopyTo(newLayers, 0);
        newLayers[currentLayers.Length] = new NoiseLayer();

        _view.UpdateNoiseLayerArray(newLayers);
    }

    private void RemoveLastNoiseLayer()
    {
        if (_view.ObjectSettings.NoiseLayers.Length <= 0)
        {
            Debug.LogWarning("Cannot remove NoiseLayer from empty array.");
            return;
        }

        var currentLayers = _view.ObjectSettings.NoiseLayers;
        if (currentLayers.Length <= 0) return;
        
        // Create a new array with the length of the current array - 1.
        var newLayers = new NoiseLayer[currentLayers.Length - 1];
   
        // Copy the new array to the current array.
        for (int layerIndex = 0; layerIndex < newLayers.Length; layerIndex++)
            newLayers[layerIndex] = currentLayers[layerIndex];

        _view.UpdateNoiseLayerArray(newLayers);
    }
}
