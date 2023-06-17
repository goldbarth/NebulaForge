using UnityEngine;

[CreateAssetMenu(fileName = "Shape Settings", menuName = "ScriptableObjects/Shape Settings")]
public class PlanetSettings : ScriptableObject
{
    [Header("Gradient Settings")]
    public Gradient HabitatColors = new();
    [Space(3)] public Gradient CandyColors = new();
    [Space(3)] public Gradient CustomColors = new();
    [Header("Surface Materials")]
    [Space(3)] public Material PlanetMaterial;
    [Space(3)] public Material OceanMaterial;
    [Space(3)] public Material CloudsMaterial;
    [Space(3)] public Material SolidMaterial;

    [Header("Shape Settings")]
    [Space] public NoiseLayer[] NoiseLayers;
}