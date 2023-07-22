// using UnityEditor;
// using UnityEngine;
//
// public class WindowModel
// {
//     private readonly ObjectSettings _data;
//     
//     private SerializedObject _serializedObject;
//     
//     private SerializedProperty _objectTypeProperty;
//     private SerializedProperty _materialProperty;
//     private SerializedProperty _resolutionProperty;
//     private SerializedProperty _radiusProperty;
//     private SerializedProperty _gradientProperty;
//     
//     private SerializedProperty _noiseLayersProperty;
//     private SerializedProperty _enabledProperty;
//     private SerializedProperty _useFirstLayerAsMaskProperty;
//     private SerializedProperty _noiseSettingsProperty;
//     
//     public WindowModel(ObjectGenerator objectGenerator)
//     {
//         _data = objectGenerator.ObjectSettings;
//     }
//     
//     public string GetAssetName()
//     {
//         return _data.name;
//     }
//     public void SetAssetName(string assetName)
//     {
//         _data.name = assetName;
//     }
//     
//     public SerializedObject SetNewSerializedObject()
//     {
//         return new SerializedObject(_data);
//     }
//     
//     public SerializedObject SetSerializedObject()
//     {
//         return _serializedObject;
//     }
//     
//     // Set Serialized Properties
//     public SerializedProperty SetSerializedObjectType()
//     {
//         return _objectTypeProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectMaterial()
//     {
//         return _materialProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectResolution()
//     {
//         return _resolutionProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectRadius()
//     {
//         return _radiusProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectGradient()
//     {
//         return _gradientProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectNoiseLayers()
//     {
//         return _noiseLayersProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectEnabled()
//     {
//         return _enabledProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectUseFirstLayerAsMask()
//     {
//         return _useFirstLayerAsMaskProperty;
//     }
//     
//     public SerializedProperty SetSerializedObjectNoiseSettings()
//     {
//         return _noiseSettingsProperty;
//     }
//     
//     
//     // Set Object Settings
//     public void SetObjectType(ObjectType objectType)
//     {
//         _data.ObjectType = objectType;
//     }
//     
//     public void SetMaterial(Material material)
//     {
//         _data.Material = material;
//     }
//     
//     public void SetResolution(int resolution)
//     {
//         _data.Resolution = resolution;
//     }
//     
//     public void SetRadius(float radius)
//     {
//         _data.Radius = radius;
//     }
//     
//     public void SetGradient(Gradient gradient)
//     {
//         _data.Gradient = gradient;
//     }
//     
//     // Set Noise Layer Settings
//     public void SetEnabled(int index, bool enabled)
//     {
//         _data.NoiseLayers[index].Enabled = enabled;
//     }
//     
//     public void SetUseFirstLayerAsMask(int index, bool useFirstLayerAsMask)
//     {
//         _data.NoiseLayers[index].UseFirstLayerAsMask = useFirstLayerAsMask;
//     }
//     
//     public void SetNoiseSettings(int index, NoiseSettings noiseSettings)
//     {
//         _data.NoiseLayers[index].NoiseSettings = noiseSettings;
//     }
//     
//     // Get Object Settings
//     public ObjectType GetObjectType()
//     {
//         return _data.ObjectType;
//     }
//     
//     public Material GetMaterial()
//     {
//         return _data.Material;
//     }
//     
//     public int GetResolution()
//     {
//         return _data.Resolution;
//     }
//     
//     public float GetRadius()
//     {
//         return _data.Radius;
//     }
//     
//     public Gradient GetGradient()
//     {
//         return _data.Gradient;
//     }
//     
//     // Get Noise Layer Settings
//     public bool GetEnabled(int index)
//     {
//         return _data.NoiseLayers[index].Enabled;
//     }
//     
//     public bool GetUseFirstLayerAsMask(int index)
//     {
//         return _data.NoiseLayers[index].UseFirstLayerAsMask;
//     }
//     
//     public NoiseSettings GetNoiseSettings(int index)
//     {
//         return _data.NoiseLayers[index].NoiseSettings;
//     }
//     
//     public int GetNoiseLayersLength()
//     {
//         return _data.NoiseLayers.Length;
//     }
// }