using UnityEngine;

public class SurfaceElevationGradient
{
    private const int TextureResolution = 50;

    private PlanetSettings _settings;
    private Texture2D _texture;
    
    private PlanetColorType _planetColorType;

    public void UpdateSettings(PlanetColorType planetColorType, PlanetSettings settings)
    {
        _planetColorType = planetColorType;
        _settings = settings;
        
        if (_texture == null)
            _texture = new Texture2D(TextureResolution, 1, TextureFormat.RGBA32, false);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        _settings.PlanetMaterial.SetVector("_Elevation_Min_Max", new Vector4(elevationMinMax.Min + 1f, elevationMinMax.Max + 1f));
    }

    public void UpdateGradient()
    {
        var colors = new Color[TextureResolution];
        for (int textureResolutionIndex = 0; textureResolutionIndex < TextureResolution; textureResolutionIndex++)
            colors[textureResolutionIndex] = GradientColor().Evaluate(textureResolutionIndex / (TextureResolution - 1f));
        
        _texture.SetPixels(colors);
        _texture.Apply();
        _settings.PlanetMaterial.SetTexture("_Planet_Texture", _texture);
    }

    private Gradient GradientColor()
    {
        switch (_planetColorType)
        {
            case PlanetColorType.Habitat:
                return _settings.HabitatColors;
            case PlanetColorType.Candy:
                return _settings.CandyColors;
            case PlanetColorType.Custom:
                return _settings.CustomColors;
            case PlanetColorType.Default:
                return new Gradient
                {
                    colorKeys = new[]
                    {
                        new GradientColorKey(Color.white, 0f),
                        new GradientColorKey(Color.white, 1f)
                    }
                };
            default:
                return new Gradient();
        }
    }
}