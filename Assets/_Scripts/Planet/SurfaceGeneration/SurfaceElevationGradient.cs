﻿using HelpersAndExtensions;
using UnityEngine;

namespace Planet.SurfaceGeneration
{
    public class SurfaceElevationGradient
    {
        private const int TextureResolution = 50;
        
        private static readonly int ElevationMinMax = Shader.PropertyToID("_Elevation_Min_Max");
    
        private ObjectGenerator _object;
        private Texture2D _texture;

        public void UpdateSettings(ObjectGenerator objectGenerator)
        {
            _object = objectGenerator;

            if (_texture == null)
                _texture = new Texture2D(TextureResolution, 1, TextureFormat.RGBA32, false);
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            if(_object.Material == null) return;
            _object.Material.SetVector(ElevationMinMax, new Vector4(elevationMinMax.Min + 1f, elevationMinMax.Max + 1f));
        }

        public void UpdateGradient()
        {
            if(_object.Material == null) return;
            
            var colors = new Color[TextureResolution];
            for (int textureResolutionIndex = 0; textureResolutionIndex < TextureResolution; textureResolutionIndex++)
                colors[textureResolutionIndex] = _object.Gradient.Evaluate(textureResolutionIndex / (TextureResolution - 1f));
        
            _texture.SetPixels(colors);
            _texture.Apply();
        
            _texture.SaveTextureToFile(_object.ObjectSettings);
            _object.Material.AddTexture(_object.ObjectSettings);
        }
    }
}