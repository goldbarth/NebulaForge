using UnityEngine;

namespace PlanetSettings.NoiseSettings
{
    [System.Serializable]
    public class SimpleNoiseSettings
    {
        [Tooltip("To reproduce generated content."), Range(0, 500)] 
        public int Seed = 0;
    
        [Tooltip("The strength of the noise. The higher the value, the more the noise will be visible (or stand out).")]
        [Range(0f, 10f)] public float NoiseStrength;
    
        [Tooltip("Number of layers for the amplitude."), Range(1, 8)] 
        public int NumberOfLayers = 1;
    
        [Tooltip("The base roughness of the noise. The higher the value, the less smooth the noise will be. (Good for eg. mountains)")]
        [Range(0f, 5f)] public float BaseRoughness = 1f;
        [Tooltip("The roughness of the noise. The higher the value, the less smooth the noise will be. (Good for eg. for less smooth surfaces)")]
        [Range(-2.5f, 10f)] public float Roughness = 2f;
    
        [Tooltip("Influences the overall appearance of the generated noise pattern.") ,Range(0f, 5f)]
        public float Persistence = 0.5f;
    
        [Tooltip("Changes the arrangement of the noise pattern.")]
        public Vector3 Center;
    
        [Tooltip("When increasing the value, the noise is pushed from the ground into the sphere."),Range(0f, 5f)]
        public float GroundLevel;
    }
}