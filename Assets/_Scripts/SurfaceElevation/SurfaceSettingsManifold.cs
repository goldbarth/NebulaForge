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
            case ObjectType.TerrestrialBody1:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Planet1Material;
                break;
            case ObjectType.TerrestrialBody2:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Planet2Material;
                break;
            case ObjectType.TerrestrialBody3:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Planet3Material;
                break;
            case ObjectType.TerrestrialBody4:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Planet4Material;
                break;
            case ObjectType.TerrestrialBody5:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Planet5Material;
                break;
            case ObjectType.TerrestrialBody6:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Planet6Material;
                break;
            case ObjectType.Shader1Material:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Shader1Material;
                break;
            case ObjectType.Shader2Material:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Shader2Material;
                break;
            case ObjectType.Shader3Material:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Shader3Material;
                break;
            case ObjectType.Shader4Material:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Shader4Material;
                break;
            case ObjectType.Shader5Material:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Shader5Material;
                break;
            case ObjectType.Shader6Material:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.Shader6Material;
                break;
            case ObjectType.SolidSphere:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.SolidMaterial;
                break;
            case ObjectType.Clouds:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.CloudsMaterial;
                break;
            case ObjectType.StylizedClouds:
                meshRenderer[cubeFaces].sharedMaterial = _settings.SurfaceSettings.StylizedCloudsMaterial;
                break;
        }
    }

    public Vector3 EvaluateShapeType(Vector3 spherePosition, ObjectType type)
    {
        var terrainElevation = _surfaceShape.TerrainElevation(spherePosition);
        var normalizedSphere = _surfaceShape.NormalizedSphere(spherePosition);
        
        ObjectType[] terrestrialObject =
        {
            ObjectType.TerrestrialBody1, ObjectType.TerrestrialBody2, ObjectType.TerrestrialBody3,
            ObjectType.TerrestrialBody4, ObjectType.TerrestrialBody5, ObjectType.TerrestrialBody6
        };
        
        foreach (var objectType in terrestrialObject)
            return type == objectType ? terrainElevation : normalizedSphere;

        return Vector3.zero;
        
        // switch (type)
        // {
        //     case ObjectType.OceanSphere:
        //         return _surfaceShape.NormalizedSphere(spherePosition);
        //     case ObjectType.SolidSphere:
        //         return _surfaceShape.NormalizedSphere(spherePosition);
        //     case ObjectType.Clouds:
        //         return _surfaceShape.NormalizedSphere(spherePosition);
        //     case ObjectType.StylizedClouds:
        //         return _surfaceShape.NormalizedSphere(spherePosition);
        //     default:
        //         return Vector3.zero;
        // }
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
    
    // sets the material for the elevation gradient texture
    public Material SetMaterial()
    {
        switch (_objectType)
        {
            case ObjectType.TerrestrialBody1:
                return _settings.SurfaceSettings.Planet1Material;
            case ObjectType.TerrestrialBody2:
                return _settings.SurfaceSettings.Planet2Material;
            case ObjectType.TerrestrialBody3:
                return _settings.SurfaceSettings.Planet3Material;
            case ObjectType.TerrestrialBody4:
                return _settings.SurfaceSettings.Planet4Material;
            case ObjectType.TerrestrialBody5:
                return _settings.SurfaceSettings.Planet5Material;
            case ObjectType.TerrestrialBody6:
                return _settings.SurfaceSettings.Planet6Material;
            default:
                return new Material(Shader.Find("Standard"));
        }
    }
}
