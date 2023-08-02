using Extensions;

namespace PlanetSettings.NoiseSettings
{
    [System.Serializable]
    public class NoiseSettings
    {
        public enum FilterType { Simple, Rigid }
        public FilterType Filter;
    
        [ConditionalHide("Filter", 0)]
        public SimpleNoiseSettings SimpleNoiseSettings;
    
        [ConditionalHide("Filter", 1)]
        public RidgidNoiseSettings RidgidNoiseSettings;
    }
}