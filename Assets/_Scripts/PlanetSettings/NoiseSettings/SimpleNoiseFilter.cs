using UnityEngine;
using Extensions;

namespace PlanetSettings.NoiseSettings
{
    public class SimpleNoiseFilter : INoiseFilter
    {
        private readonly SimpleNoiseSettings _settings;
        private readonly SimplexNoise _noise;
        
        public SimpleNoiseFilter(SimpleNoiseSettings settings)
        {
            _settings = settings;
            _noise = new SimplexNoise(_settings.Seed);
        }

        public float EvaluateNoiseValue(Vector3 spherePosition)
        {
            var frequency = _settings.BaseRoughness;
            var noiseValue = 0f;
            var amplitude = 1f;

            for (int i = 0; i < _settings.NumberOfLayers; i++)
            {
                var volume = _noise.Evaluate(spherePosition * frequency + _settings.Center);
                noiseValue += (volume + 1) * .5f * amplitude; // volume is between -1 and 1, so we add 1 and divide by 2 to get a value between 0 and 1
                frequency = frequency.MultiplyFrequencyByRoughness(_settings.Roughness); // when roughness is greater than 1, the frequency will increase each layer
                amplitude = amplitude.MultiplyAmplitudeByPersistence(_settings.Persistence); // when persistence is less than 1, the amplitude will decrease each layer
            }

            noiseValue = noiseValue.ClampValueToZeroIfBelowThreshold(_settings.GroundLevel); // if the noise value is less than the min value, set it to 0
            return noiseValue * _settings.NoiseStrength;
        }
    }
}