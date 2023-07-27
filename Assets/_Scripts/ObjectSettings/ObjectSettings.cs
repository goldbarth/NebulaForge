using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// The ObjectSettings is a ScriptableObject that holds all the settings for the ObjectGenerator.
/// It represents the Model in the MVC pattern.
/// </summary>
[CreateAssetMenu(fileName = "Object Settings", menuName = "ScriptableObjects/Object Settings")]
public class ObjectSettings : ScriptableObject
{
    [Space(3)] public ObjectType ObjectType;
    [Space(2)] public Material Material;
    
    [Header("Shape Settings")]
    [Space(2), SerializeField, Range(2, 255)] public int Resolution = 128;
    [Space(2), SerializeField, Range(0, 5000)] public float Radius = 30f;
    
    [ShowIf("HasFoldoutAccess"), Header("Surface Settings")]
    [Space(2)] public Gradient Gradient;
    
    [ShowIf("HasFoldoutAccess"), Label("Elevation Layers")] 
    [Space(2)] public NoiseLayer[] NoiseLayers;
    
    public bool HasFoldoutAccess()
    {
        return ObjectType != ObjectType.SolidSphere;
    }
}