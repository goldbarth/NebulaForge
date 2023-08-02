using UnityEngine;

namespace HelpersAndExtensions
{
    public static class CustomCalculations
    {
        public static float ClampValueToZeroIfBelowThreshold(this float noiseValue, float minValue)
        {
            return Mathf.Max(0, noiseValue - minValue);
        }
    
        public static float MultiplyFrequencyByRoughness(this float frequency, float roughness)
        {
            return frequency * roughness;
        }
    
        public static float MultiplyAmplitudeByPersistence(this float amplitude, float persistence)
        {
            return amplitude * persistence;
        }
    }
}