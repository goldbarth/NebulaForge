using Object;
using Planet;
using UnityEngine;

public class SurfaceShapeDispatcher
{
    private ObjectGenerator _object;

    public void UpdateSettings(ObjectGenerator objectGenerator)
    {
        _object = objectGenerator;
    }

    public Vector3 GetSurfaceShape(Vector3 spherePosition)
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
