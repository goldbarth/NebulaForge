# if UNITY_EDITOR

using PlanetSettings.NoiseSettings;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class DrawNoiseLayerOptions
    {
        private readonly ObjectGeneratorWindow _layout;

        public DrawNoiseLayerOptions( ObjectGeneratorWindow layout)
        {
            _layout = layout;
        }

        public void DrawNoiseLayerOptionButtons()
        {
            var buttonWidth = _layout.SetSettingsSectionWidth() * 0.5f - 10;
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
            if (_layout.ObjectSettings.NoiseLayers.Length >= 3)
            {
                Debug.LogWarning("Cannot add more than 3 NoiseLayers.");
                return;
            }

            // Create a new array with the length of the current array + 1.
            var currentLayers = _layout.ObjectSettings.NoiseLayers;
            var newLayers = new NoiseLayer[currentLayers.Length + 1];

            // Copy the current array to the new array.
            currentLayers.CopyTo(newLayers, 0);
            newLayers[currentLayers.Length] = new NoiseLayer();

            _layout.UpdateNoiseLayerArray(newLayers);
        }

        private void RemoveLastNoiseLayer()
        {
            if (_layout.ObjectSettings.NoiseLayers.Length <= 0)
            {
                Debug.LogWarning("Cannot remove NoiseLayer from empty array.");
                return;
            }

            var currentLayers = _layout.ObjectSettings.NoiseLayers;
            if (currentLayers.Length <= 0) return;
        
            // Create a new array with the length of the current array - 1.
            var newLayers = new NoiseLayer[currentLayers.Length - 1];
   
            // Copy the new array to the current array.
            for (int layerIndex = 0; layerIndex < newLayers.Length; layerIndex++)
                newLayers[layerIndex] = currentLayers[layerIndex];

            _layout.UpdateNoiseLayerArray(newLayers);
        }
    }
}

#endif
