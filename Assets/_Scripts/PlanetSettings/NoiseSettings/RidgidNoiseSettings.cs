namespace PlanetSettings.NoiseSettings
{
    [System.Serializable]
    public class RidgidNoiseSettings : SimpleNoiseSettings
    {
        public float WeightMultiplier = .8f; // with the weight multiplier we can control how much the grooves are highlighted
    }
}