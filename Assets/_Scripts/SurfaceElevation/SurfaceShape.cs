using UnityEngine;

public class SurfaceShape
{
    public MinMax ElevationMinMax;
    
    private PlanetGenerator _planet;
    private PlanetSettings _settings;
    private INoiseFilter[] _noiseFilters;

    public void UpdateSettings(PlanetSettings settings)
    {
        _noiseFilters = new INoiseFilter[settings.NoiseLayers.Length];
        _settings = settings;
        for (int noiseFilterIndex = 0; noiseFilterIndex < _noiseFilters.Length; noiseFilterIndex++)
            _noiseFilters[noiseFilterIndex] = NoiseFilterFactory.CreateNoiseFilter(settings.NoiseLayers[noiseFilterIndex].NoiseSettings);
        
        ElevationMinMax = new MinMax();
    }
    
    // makes the faces of the cube seamless when translated to a sphere
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
    
    public Vector3 CalculateSphereSurface(Vector3 spherePosition)
    {
        var firstLayerValue = 0f; // used for mask
        var elevation = 0f;

        if (_noiseFilters.Length > 0)
        {
            firstLayerValue = _noiseFilters[0].EvaluateNoiseValue(spherePosition);
            if (_settings.NoiseLayers[0].Enabled)
                elevation = firstLayerValue;
        }
        
        // exclude the first layer from the loop, because we already calculated it
        for (int noiseFilterIndex = 1; noiseFilterIndex < _noiseFilters.Length; noiseFilterIndex++)
        {
            if (_settings.NoiseLayers[noiseFilterIndex].Enabled)
            {
                // if we are using the first layer as a mask, we will use the first layer value, otherwise we will use 1, which will not affect the elevation
                var mask = _settings.NoiseLayers[noiseFilterIndex].UseFirstLayerAsMask ? firstLayerValue : 1;
                elevation += _noiseFilters[noiseFilterIndex].EvaluateNoiseValue(spherePosition) * mask; // multiply with the mask will effect that the noise will only be applied to the areas where the first layer is
            }
        }

        elevation = _settings.PlanetRadius * (1 + elevation);
        ElevationMinMax.AddValue(elevation); // add the elevation to the min max class to keep track of the min and max elevation
        return spherePosition * elevation;
    }
}