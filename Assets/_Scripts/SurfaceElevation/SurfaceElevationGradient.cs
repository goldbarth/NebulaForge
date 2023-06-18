using UnityEngine;

public class SurfaceElevationGradient
{
    private const int TextureResolution = 50;

    private SurfaceSettingsManifold _surfaceSettings;
    private Texture2D _texture;

    public void UpdateSettings(SurfaceSettingsManifold surfaceSettings)
    {
        _surfaceSettings = surfaceSettings;

        if (_texture == null)
            _texture = new Texture2D(TextureResolution, 1, TextureFormat.RGBA32, false);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        _surfaceSettings.SetMaterial().SetVector("_Elevation_Min_Max", new Vector4(elevationMinMax.Min + 1f, elevationMinMax.Max + 1f));
    }

    public void UpdateGradient()
    {
        var colors = new Color[TextureResolution];
        for (int textureResolutionIndex = 0; textureResolutionIndex < TextureResolution; textureResolutionIndex++)
            colors[textureResolutionIndex] = _surfaceSettings.GradientColor().Evaluate(textureResolutionIndex / (TextureResolution - 1f));
        
        _texture.SetPixels(colors);
        _texture.Apply();
            _surfaceSettings.SetMaterial().SetTexture("_Planet_Texture", _texture);
    }
}