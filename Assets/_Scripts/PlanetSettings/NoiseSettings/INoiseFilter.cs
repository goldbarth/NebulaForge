using UnityEngine;

namespace PlanetSettings.NoiseSettings
{
    public interface INoiseFilter
    {
        float EvaluateNoiseValue(Vector3 spherePosition);
    }
}