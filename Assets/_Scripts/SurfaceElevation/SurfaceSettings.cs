using UnityEngine;

[System.Serializable]
public class SurfaceSettings
{
    [Header("Surface Gradient")]
    public Gradient HabitatColors = new();
    [Space(3)] public Gradient CandyColors = new();
    [Space(3)] public Gradient CustomColors = new();
    
    [Header("Surface Materials")]
    [Space(3)] public Material Planet1Material;
    [Space(3)] public Material Planet2Material;
    [Space(3)] public Material Planet3Material;
    [Space(3)] public Material Planet4Material;
    [Space(3)] public Material Planet5Material;
    [Space(3)] public Material Planet6Material;
    [Space(3)] public Material Shader1Material;
    [Space(3)] public Material Shader2Material;
    [Space(3)] public Material Shader3Material;
    [Space(3)] public Material Shader4Material;
    [Space(3)] public Material Shader5Material;
    [Space(3)] public Material Shader6Material;
    [Space(3)] public Material OceanMaterial;
    [Space(3)] public Material CloudsMaterial;
    [Space(3)] public Material StylizedCloudsMaterial;
    [Space(3)] public Material SolidMaterial;
}