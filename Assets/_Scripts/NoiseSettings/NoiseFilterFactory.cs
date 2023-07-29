using UnityEngine;

public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        
        Debug.Log("Filter: " + settings.Filter);
        switch (settings.Filter)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.SimpleNoiseSettings);
            case NoiseSettings.FilterType.Rigid:
                return new RidgidNoiseFilter(settings.RidgidNoiseSettings);
            default:
                return null;
        }
    }
}