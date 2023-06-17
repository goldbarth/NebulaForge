using UnityEngine;

public class SurfaceShape
{
    public MinMax ElevationMinMax;
    
    private INoiseFilter[] _noiseFilters;
    
    private PlanetGenerator _planetGenerator;
    private PlanetSettings _settings;

    public void UpdateSettings(PlanetSettings settings, PlanetGenerator planetGenerator)
    {
        _noiseFilters = new INoiseFilter[settings.NoiseLayers.Length];
        ElevationMinMax = new MinMax();
        
        _planetGenerator = planetGenerator;
        _settings = settings;
        
        for (int layerIndex = 0; layerIndex < NoiseFilterCount(); layerIndex++)
            _noiseFilters[layerIndex] = NoiseFilterFactory.CreateNoiseFilter(settings.NoiseLayers[layerIndex].NoiseSettings);
    }
    
    // makes the mesh face edges seamless when translated to a sphere
    public Vector3 CalculateSeamlessEdges(Vector3 cubePosition)
    {
        var x2 = cubePosition.x * cubePosition.x;
        var y2 = cubePosition.y * cubePosition.y;
        var z2 = cubePosition.z * cubePosition.z;
    
        var xPrime = cubePosition.x * Mathf.Sqrt(1f - y2 * .5f - z2 * .5f + y2 * z2 / 3f);
        var yPrime = cubePosition.y * Mathf.Sqrt(1f - z2 * .5f - x2 * .5f + z2 * x2 / 3f);
        var zPrime = cubePosition.z * Mathf.Sqrt(1f - x2 * .5f - y2 * .5f + x2 * y2 / 3f);
    
        return new Vector3(xPrime, yPrime, zPrime);
    }
    
    public Vector3 EvaluateShapeType(Vector3 spherePosition, PlanetShapeType shapeType)
    {
        switch (shapeType)
        {
            case PlanetShapeType.WithOutElevation:
                return spherePosition.normalized * PlanetRadius();
            case PlanetShapeType.WithElevation:
                return CalculateElevation(spherePosition);
            default:
                return Vector3.zero;
        }
    }

    private Vector3 CalculateElevation(Vector3 spherePosition)
    {
        var firstLayerValue = 0f; // first layer is used for mask
        var elevation = 0f;

        if (NoiseFilterCount() > 0)
        {
            firstLayerValue = _noiseFilters[0].EvaluateNoiseValue(spherePosition);
            if (_settings.NoiseLayers[0].Enabled)
                elevation = firstLayerValue;
        }
        
        // exclude the first layer from the loop, because we already calculated it
        for (int layerIndex = 1; layerIndex < NoiseFilterCount(); layerIndex++)
        {
            if (_settings.NoiseLayers[layerIndex].Enabled)
            {
                // we use the first layer as a mask for the other layers
                var mask = _settings.NoiseLayers[layerIndex].UseFirstLayerAsMask ? firstLayerValue : 1;
                elevation += _noiseFilters[layerIndex].EvaluateNoiseValue(spherePosition) * mask;
            }
        }

        elevation = PlanetRadius() * (1 + elevation);
        ElevationMinMax.AddValue(elevation);
        return spherePosition * elevation;
    }

    private int NoiseFilterCount()
    {
        return _noiseFilters.Length;
    }
    
    private float PlanetRadius()
    {
        return _planetGenerator.PlanetRadius;
    }
}