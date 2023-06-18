using UnityEngine;

[CreateAssetMenu(fileName = "Shape Settings", menuName = "ScriptableObjects/Shape Settings")]
public class PlanetSettings : ScriptableObject
{
    [Header("Object Settings")]
    [Space] public SurfaceSettings SurfaceSettings;

    [Header("Shape Settings")]
    [Space] public NoiseLayer[] NoiseLayers;
}