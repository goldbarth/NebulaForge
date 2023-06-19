using UnityEngine;

[CreateAssetMenu(fileName = "Shape Settings", menuName = "ScriptableObjects/Shape Settings")]
public class PlanetSettings : ScriptableObject
{
    [Space] public NoiseLayer[] NoiseLayers;
}