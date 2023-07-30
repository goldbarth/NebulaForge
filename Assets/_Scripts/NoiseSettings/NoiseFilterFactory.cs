public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
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