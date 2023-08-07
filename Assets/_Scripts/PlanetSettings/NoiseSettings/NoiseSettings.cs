using Extensions;
using UnityEngine;

namespace PlanetSettings.NoiseSettings
{
    [System.Serializable]
    public class NoiseSettings
    {
        public enum FilterType { Simple, Rigid }
        [Tooltip("Choose the type of noise filter. Simple is good for eg. terrain and hills, Ridgid is good for eg. mountains(sharper edges).")]
        public FilterType Filter;
    
        [ConditionalHide("Filter", 0)]
        public SimpleNoiseSettings SimpleNoiseSettings;
    
        [ConditionalHide("Filter", 1)]
        public RidgidNoiseSettings RidgidNoiseSettings;
    }
}