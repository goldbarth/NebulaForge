using UnityEngine;

public class SurfaceSettingsDispatcher
{
    private ObjectGenerator _object;

    public void UpdateObjectSettings(ObjectGenerator objectGenerator)
    {
        _object = objectGenerator;
    }

    public Vector3 EvaluateShapeType(Vector3 spherePosition)
    {
        var terrainElevation = _object.SurfaceShape.TerrainElevation(spherePosition);
        var normalizedSphere = _object.SurfaceShape.NormalizedSphere(spherePosition);
        
        switch (_object.ObjectType)
        {
            case ObjectType.TerrestrialBody:
                return terrainElevation;
            case ObjectType.SolidSphere:
                return normalizedSphere;
            default:
                return Vector3.zero;
        }
    }
}
