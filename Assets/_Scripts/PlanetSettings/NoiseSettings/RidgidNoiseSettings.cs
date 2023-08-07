using UnityEngine;

namespace PlanetSettings.NoiseSettings
{
    [System.Serializable]
    public class RidgidNoiseSettings : SimpleNoiseSettings
    {
        [Tooltip("The weight multiplier is used to control the weight of the grooves.")]
        public float WeightMultiplier = 0.8f;
    }
}