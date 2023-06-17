using UnityEngine;

[CreateAssetMenu(fileName = "Shape Settings", menuName = "ScriptableObjects/Shape Settings")]
public class PlanetSettings : ScriptableObject
{
    [Header("Color Settings")]
    public Gradient Gradient = new();
    public Material PlanetMaterial;
    public Material OceanMaterial;
    
    [Header("Shape Settings")]
    [Space] public NoiseLayer[] NoiseLayers;
}