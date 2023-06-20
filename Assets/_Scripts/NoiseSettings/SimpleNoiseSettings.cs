using UnityEngine;

[System.Serializable]
public class SimpleNoiseSettings
{
    [Tooltip("To reproduce generated content."), Range(0, 500)] 
    public int Seed = 0;
    
    [Range(0, 10)] public float NoiseStrength;
    
    [Tooltip("Number of layers for the amplitude."), Range(1, 8)] 
    public int NumberOfLayers = 1;
    
    [Range(0, 5)] public float BaseRoughness = 1f;
    [Range(-2.5f, 10f)] public float Roughness = 2f;
    
    [Tooltip("Amplitude will decrease by half each layer.The amplitude will decrease each layer.") ,Range(0, 5)]
    public float Persistence = .5f;
    public Vector3 Center;
    [Tooltip("The minimum value allows us to push the noise back into the sphere of the planet."),Range(0, 5)]
    public float MinValue;
}