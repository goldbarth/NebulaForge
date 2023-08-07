using PlanetSettings.NoiseSettings;
using NaughtyAttributes;
using UnityEngine;
using System;

namespace PlanetSettings
{
    /// <summary>
    /// The ObjectSettings is a ScriptableObject that holds all the settings for the ObjectGenerator.
    /// It represents the Model in the MVP pattern.
    /// </summary>
    [CreateAssetMenu(fileName = "Object Settings", menuName = "ScriptableObjects/Object Settings")]
    public class ObjectSettings : ScriptableObject
    {
        [Tooltip("Choose the type of object. SolidSphere is good for eg. shaders and plain surfaces, TerrestrialBody is good for eg. a planets (with habitat) and elevation.")]
        [Space(3)] public ObjectType ObjectType;
        [Space(2)] public Material Material;
    
        [Tooltip("The resolution of the object. The higher the resolution, the more vertices the object will have.")]
        [Space(2), SerializeField, Range(2, 255)] public int Resolution = 128;
        [Tooltip("The radius of the object. The higher the radius, the bigger the object will be.")]
        [Space(2), SerializeField, Range(0, 1000)] public float Radius = 30f;
        
        [Tooltip("The gradient of the object. The gradient is used to color the object from center to the highest point.")]
        [Space(2)] public Gradient Gradient;
    
        [Label("Elevation Layers")] 
        [Space(2)] public NoiseLayer[] NoiseLayers;
    
        
        // Model Methods and Events:
    
        public event Action OnSettingsChangedReady;

        public void UpdateSettings(ObjectSettings objectSettings)
        {
            ObjectType = objectSettings.ObjectType;
            Material = objectSettings.Material;
            Resolution = objectSettings.Resolution;
            Radius = objectSettings.Radius;
            Gradient = objectSettings.Gradient;
            NoiseLayers = objectSettings.NoiseLayers;
            OnSettingsChangedReady?.Invoke();
        }
    }
}