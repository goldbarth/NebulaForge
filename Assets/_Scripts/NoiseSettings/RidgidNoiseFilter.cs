using UnityEngine;

public class RidgidNoiseFilter : INoiseFilter
{
    private readonly RidgidNoiseSettings _settings;
    private readonly SimplexNoise _noise;
        
    public RidgidNoiseFilter(RidgidNoiseSettings settings)
    {
        _settings = settings;
        _noise = new SimplexNoise(_settings.Seed);
    }

    public float EvaluateNoiseValue(Vector3 spherePosition)
    {
        var frequency = _settings.BaseRoughness;
        var noiseValue = 0f;
        var amplitude = 1f;
        var weight = 1f;

        for (int layerIndex = 0; layerIndex < _settings.NumberOfLayers; layerIndex++)
        {
            var volume = 1 - Mathf.Abs(_noise.Evaluate(spherePosition * frequency + _settings.Center));
            volume *= volume; // the squaring makes the grooves more pronounced
            volume *= weight; // with the weight multiplier we can control how much the grooves are highlighted
            weight = Mathf.Clamp01(volume * _settings.WeightMultiplier); // lower regions are less detailed compared to higher regions
            
            noiseValue += volume * amplitude;
            frequency = frequency.MultiplyFrequencyByRoughness(_settings.Roughness); // when roughness is greater than 1, the frequency will increase each layer
            amplitude = amplitude.MultiplyAmplitudeByPersistence(_settings.Persistence); // when persistence is less than 1, the amplitude will decrease each layer
        }
            
        noiseValue = noiseValue.ClampValueToZeroIfBelowThreshold(_settings.GroundLevel); // if the noise value is less than the min value, set it to 0
        return noiseValue * _settings.NoiseStrength;
    }
}