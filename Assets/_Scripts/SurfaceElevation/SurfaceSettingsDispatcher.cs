using UnityEngine;

public class SurfaceSettingsDispatcher
{
    private SurfaceShape _surfaceShape;
    private ObjectType _objectType;

    public void UpdateObjectSettings(ObjectType objectType, SurfaceShape surfaceShape)
    {
        _surfaceShape = surfaceShape;
        _objectType = objectType;
    }

    public Vector3 EvaluateShapeType(Vector3 spherePosition)
    {
        var terrainElevation = _surfaceShape.TerrainElevation(spherePosition);
        var normalizedSphere = _surfaceShape.NormalizedSphere(spherePosition);
        switch (_objectType)
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
