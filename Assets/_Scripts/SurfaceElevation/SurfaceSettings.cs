using UnityEngine;

[System.Serializable]
public class SurfaceSettings
{
    [Header("Surface Gradient")]
    public Gradient HabitatColors = new();
    [Space(3)] public Gradient CandyColors = new();
    [Space(3)] public Gradient CustomColors = new();
    
    [Header("Surface Materials")]
    [Space(3)] public Material PlanetMaterial;
    [Space(3)] public Material Planet2Material;
    [Space(3)] public Material OceanMaterial;
    [Space(3)] public Material CloudsMaterial;
    [Space(3)] public Material SolidMaterial;
}