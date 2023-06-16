using UnityEngine;

[CreateAssetMenu(fileName = "Shape Settings", menuName = "ScriptableObjects/Shape Settings")]
public class PlanetSettings : ScriptableObject
{
    [Header("Color Settings")]
    public Gradient Gradient = new();
    public Material PlanetMaterial;
    [Header("Shape Settings")]
    [Tooltip("Scale the Planet from 0.5 to 10"), Range(0.5f, 10f)] 
    public float PlanetRadius = 1f;
    [Space] public NoiseLayer[] NoiseLayers;
}