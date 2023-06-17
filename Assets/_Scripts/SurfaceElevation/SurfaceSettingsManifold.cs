using UnityEngine;

public class SurfaceSettingsManifold
{
    private SurfaceShape _surfaceShape;
    private PlanetSettings _settings;
    private PlanetColorType _planetColorType;
    private ObjectType _objectType;

    public void UpdateObjectSettings(ObjectType objectType, PlanetColorType planetColorType, SurfaceShape surfaceShape, PlanetSettings planetSettings)
    {
        _settings = planetSettings;
        _planetColorType = planetColorType;
        _surfaceShape = surfaceShape;
        _objectType = objectType;
    }

    public void AddShapeMaterial(MeshRenderer[] meshRenderer, int cubeFaces)
    {
        switch (_objectType)
        {
            case ObjectType.OceanSphere:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.OceanMaterial;
                break;
            case ObjectType.TerrestrialBody:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.PlanetMaterial;
                break;
            case ObjectType.SolidSphere:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.SolidMaterial;
                break;
            case ObjectType.Clouds:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.CloudsMaterial;
                break;
        }
    }

    public Vector3 EvaluateShapeType(Vector3 spherePosition, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.OceanSphere:
                return _surfaceShape.NormalizedSphere(spherePosition);
            case ObjectType.SolidSphere:
                return _surfaceShape.NormalizedSphere(spherePosition);
            case ObjectType.Clouds:
                return _surfaceShape.NormalizedSphere(spherePosition);
            case ObjectType.TerrestrialBody:
                return _surfaceShape.TerrainElevation(spherePosition);
            default:
                return Vector3.zero;
        }
    }

    public Gradient GradientColor()
    {
        switch (_planetColorType)
        {
            case PlanetColorType.Habitat:
                return _settings.SurfaceSettings.HabitatColors;
            case PlanetColorType.Candy:
                return _settings.SurfaceSettings.CandyColors;
            case PlanetColorType.Custom:
                return _settings.SurfaceSettings.CustomColors;
            case PlanetColorType.Default:
                return new Gradient();
            default:
                return new Gradient();
        }
    }
}
