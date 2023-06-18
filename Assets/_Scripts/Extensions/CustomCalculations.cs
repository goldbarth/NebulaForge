using UnityEngine;

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

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}